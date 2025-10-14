using System;
using System.Linq;
using System.Threading.Tasks;
using db.Models;
using db.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace wServer.realm
{
    // Repository-based adapter for legacy Database calls
    public class DatabaseAdapter : IDisposable
    {
        private readonly IServiceProvider _services;

        public DatabaseAdapter(IServiceProvider services)
        {
            _services = services;
        }

        // Execute legacy action pattern (sync or async) in a unified way
        public Task DoActionAsync(Action<DatabaseAdapter> action)
        {
            action(this);
            return Task.CompletedTask;
        }

        public async Task DoActionAsync(Func<DatabaseAdapter, Task> action)
        {
            await action(this).ConfigureAwait(false);
        }

        public async Task<int> UpdateCreditAsync(Account account, int delta)
        {
            using var scope = _services.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var stats = await uow.Stats.GetByAccountIdAsync(account.Id).ConfigureAwait(false);
            if (stats == null)
            {
                stats = new Stat { AccountId = account.Id, Credits = 0 };
                await uow.Stats.AddAsync(stats).ConfigureAwait(false);
            }

            stats.Credits += delta;
            await uow.Stats.UpdateAsync(stats).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            // Update account's cached credits value
            account.Credits = stats.Credits;
            return stats.Credits;
        }

        // Legacy sync wrapper for compatibility
        public int UpdateCredit(Account account, int delta)
        {
            return UpdateCreditAsync(account, delta).GetAwaiter().GetResult();
        }

        public async Task UpdateLastSeenAsync(string accountId, int characterId, string worldName)
        {
            using var scope = _services.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            if (!long.TryParse(accountId, out var accId)) return;

            var chr = await uow.Characters.GetByCharacterIdAsync(accId, characterId).ConfigureAwait(false);
            if (chr == null) return;

            chr.LastSeen = DateTime.Now;
            chr.LastLocation = worldName ?? string.Empty;
            await uow.Characters.UpdateAsync(chr).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        // Legacy sync wrapper for compatibility
        public void UpdateLastSeen(string accountId, int characterId, string worldName)
        {
            UpdateLastSeenAsync(accountId, characterId, worldName).GetAwaiter().GetResult();
        }

        public async Task SaveCharacterAsync(Account account, Char character)
        {
            using var scope = _services.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            if (!long.TryParse(account.AccountId, out var accId)) return;

            var chr = await uow.Characters.GetByCharacterIdAsync(accId, character.CharacterId).ConfigureAwait(false);
            var isNew = false;
            if (chr == null)
            {
                chr = new Character
                {
                    AccountId = accId,
                    CharacterId = character.CharacterId
                };
                isNew = true;
            }

            chr.CharacterType = character.ObjectType;
            chr.Level = (byte)Math.Clamp(character.Level, 1, 100);
            chr.Experience = character.Exp;
            chr.Fame = character.CurrentFame;
            chr.Items = character.Equipment ?? Array.Empty<int>();
            chr.HpPotions = character.HealthStackCount;
            chr.MpPotions = character.MagicStackCount;
            chr.Hp = character.HitPoints;
            chr.Mp = character.MagicPoints;
            chr.Stats = new CharacterStats()
            {
                Attack = character.Attack,
                Defense = character.Defense,
                Speed = character.Speed,
                Dexterity = character.Dexterity,
                Vitality = character.HpRegen,
                Wisdom = character.MpRegen,
                MaxHitPoints = character.MaxHitPoints,
                MaxMagicPoints = character.MaxMagicPoints
            };
            chr.Dead = character.Dead;
            chr.Tex1 = character.Tex1;
            chr.Tex2 = character.Tex2;
            chr.PetItemType = character.Pet?.ItemType ?? 0;
            
            if (character.Pet != null)
            {
                chr.Pet = new Pet()
                {
                    PetId = character.Pet.Id,
                    AccountId = accId,
                    ObjectType = character.Pet.ItemType,
                    Rarity = (byte)character.Pet.Rarity,
                    MaxAbilityPower = (byte)character.Pet.MaxAbilityPower,
                    Ability1Type = character.Pet.Abilities[0].Type,
                    Ability2Type = character.Pet.Abilities[1].Type,
                    Ability3Type = character.Pet.Abilities[2].Type,
                    Skin = character.Pet.Skin,
                    Name = character.Pet.SkinName ?? string.Empty
                };
            }

            chr.HasBackpack = character.HasBackpack != 0;
            chr.Skin = character.Skin;
            chr.XpBoosterTime = character.XpTimer;
            chr.LdTimer = character.LDTimer;
            chr.LtTimer = character.LTTimer;
            chr.FameStats = JsonConvert.SerializeObject(character.FameStats ?? new FameStats()); 
            chr.TotalFame = Math.Max(chr.TotalFame, character.CurrentFame);
            chr.LastSeen = DateTime.Now;

            if (isNew)
                await uow.Characters.AddAsync(chr).ConfigureAwait(false);
            else
                await uow.Characters.UpdateAsync(chr).ConfigureAwait(false);

            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        // Legacy sync wrapper for compatibility
        public void SaveCharacter(Account account, Char character)
        {
            SaveCharacterAsync(account, character).GetAwaiter().GetResult();
        }

        public async Task UnlockAccountAsync(Account account)
        {
            using var scope = _services.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var acc = await uow.Accounts.GetByIdAsync((int)account.Id).ConfigureAwait(false);
            if (acc == null) return;

            acc.AccountInUse = false;
            uow.Accounts.Update(acc);
            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        // Legacy sync wrapper for compatibility
        public void UnlockAccount(Account account)
        {
            UnlockAccountAsync(account).GetAwaiter().GetResult();
        }

        public async Task SaveChestAsync(string accountId, VaultChest chest)
        {
            using var scope = _services.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            if (!long.TryParse(accountId, out var accId)) return;

            var v = await uow.Vaults.GetByChestIdAsync(accId, chest.ChestId).ConfigureAwait(false);
            if (v == null)
            {
                v = new db.Models.Vault
                {
                    AccountId = accId,
                    ChestId = chest.ChestId,
                    Items = chest._Items ?? string.Empty,
                    ChestType = 0
                };
                await uow.Vaults.AddAsync(v).ConfigureAwait(false);
            }
            else
            {
                v.Items = chest._Items ?? string.Empty;
                await uow.Vaults.UpdateAsync(v).ConfigureAwait(false);
            }

            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        // Legacy sync wrapper for compatibility
        public void SaveChest(string accountId, VaultChest chest)
        {
            SaveChestAsync(accountId, chest).GetAwaiter().GetResult();
        }

        public async Task<string> GenerateGiftcodeAsync(string contentJson, string accountId)
        {
            using var scope = _services.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var code = Utils.GenerateRandomString(10);
            var gift = new GiftCode
            {
                Code = code,
                Content = contentJson,
                AccId = ulong.TryParse(accountId, out var aid) ? aid : 0UL,
                CreatedAt = DateTime.UtcNow,
                RedeemedAt = null
            };
            await uow.GiftCodes.AddAsync(gift).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);
            return code;
        }

        // Legacy sync wrapper for compatibility
        public string GenerateGiftcode(string contentJson, string accountId)
        {
            return GenerateGiftcodeAsync(contentJson, accountId).GetAwaiter().GetResult();
        }

        public async Task CreateChest(Account account)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var vault = new db.Models.Vault
            {
                AccountId = account.Id, // Use Id instead of AccountId
                Items = "[]" // Use Items instead of Chests
            };

            await unitOfWork.Vaults.AddAsync(vault).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<GuildEntity> GetGuild(int guildId)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            return await unitOfWork.Guilds.GetByIdAsync(guildId).ConfigureAwait(false);
        }

        public async Task<GuildEntity> GetGuild(string name)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            return await unitOfWork.Guilds.GetByNameAsync(name).ConfigureAwait(false);
        }

        public async Task<GuildEntity> CreateGuild(string name, Account account)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var guild = new GuildEntity
            {
                Name = name,
                Level = 1,
                Fame = 0,
                TotalFame = 0
            };

            await unitOfWork.Guilds.AddAsync(guild).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return guild;
        }

        public async Task ChangeGuild(Account account, GuildEntity guild, bool remove = false, int rank = 0)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            if (remove)
            {
                account.GuildId = 0;
                account.GuildRank = 0;
            }
            else
            {
                account.GuildId = guild.Id;
                account.GuildRank = (byte)rank;
            }

            unitOfWork.Accounts.Update(account);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        // Legacy compatibility signature used across handlers - converted to async
        public async Task<GuildEntity> ChangeGuildAsync(Account account, int guildId, int rank, int fame, bool remove)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            GuildEntity guild = null;
            if (!remove)
            {
                guild = await unitOfWork.Guilds.GetByIdAsync(guildId).ConfigureAwait(false);
            }

            if (remove)
            {
                account.GuildId = 0;
                account.GuildRank = 0;
            }
            else
            {
                account.GuildId = guild?.Id ?? 0;
                account.GuildRank = (byte)rank;
            }

            unitOfWork.Accounts.Update(account);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return guild;
        }

        // Legacy sync wrapper for compatibility
        public GuildEntity ChangeGuild(Account account, int guildId, int rank, int fame, bool remove)
        {
            return ChangeGuildAsync(account, guildId, rank, fame, remove).GetAwaiter().GetResult();
        }

        public async Task<Account> GetAccount(int accountId)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            return await unitOfWork.Accounts.GetByIdAsync(accountId).ConfigureAwait(false);
        }

        public async Task<Account> GetAccountByName(string name)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            return await unitOfWork.Accounts.FirstOrDefaultAsync(a => a.Name == name).ConfigureAwait(false);
        }

        // Compatibility overload used by legacy code that passed GameData; ignored here
        public Task<Account> GetAccountByName(string name, object _)
            => GetAccountByName(name);

        // Compatibility method name used in a few places: GetAccount(string, gameData)
        public Task<Account> GetAccount(string name, object _)
            => GetAccountByName(name);

        public async Task UpdateFameAsync(Account account, int fame)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            account.Fame += fame;
            account.TotalFame += fame;

            unitOfWork.Accounts.Update(account);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateFortuneTokenAsync(Account account, int tokens)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            account.FortuneTokens += tokens;

            unitOfWork.Accounts.Update(account);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }


        public async Task<Pet> GetPet(int petId)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            return await unitOfWork.Pets.GetByIdAsync(petId).ConfigureAwait(false);
        }

        public async Task<Pet> CreatePet(Account account, int petType)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var pet = new Pet
            {
                AccountId = account.Id,
                ObjectType = petType, // Use ObjectType instead of PetType
                Rarity = 0,
                MaxAbilityPower = 30, // Set MaxAbilityPower instead of MaxLevel
                Ability1Type = 0, // Use Ability1Type instead of FirstAbility
                Ability2Type = 0, // Use Ability2Type instead of SecondAbility
                Ability3Type = 0, // Use Ability3Type instead of ThirdAbility
                Skin = 0,
                Name = "Pet"
            };

            await unitOfWork.Pets.AddAsync(pet).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return pet;
        }

        // Compatibility overloads used in old code paths
        public int UpdateFame(Account account, int delta)
        {
            // Provide sync wrapper for legacy code expecting int return
            UpdateFameAsync(account, delta).GetAwaiter().GetResult();
            return account.Stats?.Fame ?? 0;
        }

        public int UpdateFortuneToken(Account account, int delta)
        {
            UpdateFortuneTokenAsync(account, delta).GetAwaiter().GetResult();
            return account.FortuneTokens;
        }

        public async Task<int> GetNextPetId()
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            // TODO: implement based on repository once available. For now, try max + 1.
            var pets = await unitOfWork.Pets.GetAllAsync().ConfigureAwait(false);
            var max = pets.Any() ? pets.Max(p => (int)p.PetId) : 0;
            return max + 1;
        }

        public async Task SaveBackpacks(Account account, string backpacks)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            account.Backpacks = backpacks;

            unitOfWork.Accounts.Update(account);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        // Legacy CreateQuery method - returns a placeholder queryable for raw SQL compatibility
        public IQueryable<T> CreateQuery<T>() where T : class
        {
            // This is a compatibility method for legacy raw SQL queries
            // In practice, these should be converted to repository calls
            throw new NotImplementedException("CreateQuery is deprecated - use repository methods instead");
        }

        public void Dispose()
        {
            // Nothing to dispose for adapter
        }
    }
}