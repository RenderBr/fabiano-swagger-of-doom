-- Modern MySQL Database Schema
-- Compatible with MySQL 8.0+
-- Updated: October 2025

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

--
-- Database: `rotmgprod`
--
CREATE DATABASE IF NOT EXISTS `rotmgprod` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `rotmgprod`;

-- --------------------------------------------------------

--
-- Table structure for `accounts`
--

CREATE TABLE IF NOT EXISTS `accounts` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `uuid` VARCHAR(128) NOT NULL,
  `password` VARCHAR(256) NOT NULL,
  `name` VARCHAR(64) NOT NULL DEFAULT 'DEFAULT',
  `rank` TINYINT UNSIGNED NOT NULL DEFAULT '0',
  `namechosen` BOOLEAN NOT NULL DEFAULT FALSE,
  `verified` BOOLEAN NOT NULL DEFAULT TRUE,
  `guild` INT UNSIGNED NOT NULL DEFAULT 0,
  `guildRank` TINYINT UNSIGNED NOT NULL DEFAULT 0,
  `guildFame` INT UNSIGNED NOT NULL DEFAULT '0',
  `lastip` VARCHAR(45) NOT NULL DEFAULT '',
  `vaultCount` TINYINT UNSIGNED NOT NULL DEFAULT '1',
  `maxCharSlot` TINYINT UNSIGNED NOT NULL DEFAULT '2',
  `regTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `guest` BOOLEAN NOT NULL DEFAULT FALSE,
  `banned` BOOLEAN NOT NULL DEFAULT FALSE,
  `publicMuledump` BOOLEAN NOT NULL DEFAULT TRUE,
  `muted` BOOLEAN NOT NULL DEFAULT FALSE,
  `prodAcc` BOOLEAN NOT NULL DEFAULT FALSE,
  `locked` TEXT,
  `ignored` TEXT,
  `gifts` TEXT,
  `isAgeVerified` BOOLEAN NOT NULL DEFAULT FALSE,
  `petYardType` TINYINT UNSIGNED NOT NULL DEFAULT '1',
  `ownedSkins` TEXT,
  `authToken` VARCHAR(128) NOT NULL DEFAULT '',
  `acceptedNewTos` BOOLEAN NOT NULL DEFAULT TRUE,
  `lastSeen` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `accountInUse` BOOLEAN NOT NULL DEFAULT FALSE,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uuid` (`uuid`),
  KEY `idx_guild` (`guild`),
  KEY `idx_lastip` (`lastip`),
  KEY `idx_banned` (`banned`),
  KEY `idx_lastseen` (`lastSeen`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `arenalb`
--

CREATE TABLE IF NOT EXISTS `arenalb` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `wave` SMALLINT UNSIGNED NOT NULL,
  `accid` BIGINT UNSIGNED NOT NULL,
  `charid` INT UNSIGNED NOT NULL,
  `petid` INT UNSIGNED DEFAULT NULL,
  `time` VARCHAR(256) NOT NULL,
  `date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_wave` (`wave`),
  KEY `idx_accid` (`accid`),
  KEY `idx_date` (`date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `backpacks`
--

CREATE TABLE IF NOT EXISTS `backpacks` (
  `accId` BIGINT UNSIGNED NOT NULL,
  `charId` INT UNSIGNED NOT NULL,
  `items` JSON NOT NULL,
  PRIMARY KEY (`accId`, `charId`),
  KEY `idx_charid` (`charId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `boards`
--

CREATE TABLE IF NOT EXISTS `boards` (
  `guildId` INT UNSIGNED NOT NULL,
  `text` VARCHAR(1024) NOT NULL,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`guildId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `characters`
--

CREATE TABLE IF NOT EXISTS `characters` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `accId` BIGINT UNSIGNED NOT NULL,
  `charId` SMALLINT UNSIGNED NOT NULL,
  `charType` SMALLINT UNSIGNED NOT NULL DEFAULT '782',
  `level` TINYINT UNSIGNED NOT NULL DEFAULT '1',
  `exp` INT UNSIGNED NOT NULL DEFAULT '0',
  `fame` INT UNSIGNED NOT NULL DEFAULT '0',
  `items` JSON NOT NULL,
  `hpPotions` SMALLINT UNSIGNED NOT NULL DEFAULT '0',
  `mpPotions` SMALLINT UNSIGNED NOT NULL DEFAULT '0',
  `hp` SMALLINT UNSIGNED NOT NULL DEFAULT '1',
  `mp` SMALLINT UNSIGNED NOT NULL DEFAULT '1',
  `stats` JSON NOT NULL,
  `dead` BOOLEAN NOT NULL DEFAULT FALSE,
  `tex1` SMALLINT UNSIGNED NOT NULL DEFAULT '0',
  `tex2` SMALLINT UNSIGNED NOT NULL DEFAULT '0',
  `pet` INT UNSIGNED NOT NULL DEFAULT '0',
  `petId` INT UNSIGNED NOT NULL DEFAULT '0',
  `hasBackpack` BOOLEAN NOT NULL DEFAULT FALSE,
  `skin` SMALLINT UNSIGNED NOT NULL DEFAULT '0',
  `xpBoosterTime` INT UNSIGNED NOT NULL DEFAULT '0',
  `ldTimer` INT UNSIGNED NOT NULL DEFAULT '0',
  `ltTimer` INT UNSIGNED NOT NULL DEFAULT '0',
  `fameStats` JSON,
  `createTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `deathTime` DATETIME NULL DEFAULT NULL,
  `totalFame` INT UNSIGNED NOT NULL DEFAULT '0',
  `lastSeen` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lastLocation` VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `idx_accid` (`accId`),
  KEY `idx_dead` (`dead`),
  KEY `idx_accid_dead` (`accId`, `dead`),
  KEY `idx_lastseen` (`lastSeen`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `classstats`
--

CREATE TABLE IF NOT EXISTS `classstats` (
  `accId` BIGINT UNSIGNED NOT NULL,
  `objType` SMALLINT UNSIGNED NOT NULL,
  `bestLv` TINYINT UNSIGNED NOT NULL DEFAULT '1',
  `bestFame` INT UNSIGNED NOT NULL DEFAULT '0',
  PRIMARY KEY (`accId`, `objType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `dailyquests`
--

CREATE TABLE IF NOT EXISTS `dailyquests` (
  `accId` BIGINT UNSIGNED NOT NULL,
  `goals` JSON NOT NULL,
  `tier` TINYINT UNSIGNED NOT NULL DEFAULT '1',
  `time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`accId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `death`
--

CREATE TABLE IF NOT EXISTS `death` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `accId` BIGINT UNSIGNED NOT NULL,
  `chrId` INT UNSIGNED NOT NULL,
  `name` VARCHAR(64) NOT NULL DEFAULT 'DEFAULT',
  `charType` SMALLINT UNSIGNED NOT NULL DEFAULT '782',
  `tex1` SMALLINT UNSIGNED NOT NULL DEFAULT '0',
  `tex2` SMALLINT UNSIGNED NOT NULL DEFAULT '0',
  `skin` SMALLINT UNSIGNED NOT NULL DEFAULT '0',
  `items` JSON NOT NULL,
  `fame` INT UNSIGNED NOT NULL DEFAULT '0',
  `exp` INT UNSIGNED NOT NULL DEFAULT '0',
  `fameStats` JSON,
  `totalFame` INT UNSIGNED NOT NULL DEFAULT '0',
  `firstBorn` BOOLEAN NOT NULL DEFAULT FALSE,
  `killer` VARCHAR(128) NOT NULL,
  `time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_accid` (`accId`),
  KEY `idx_time` (`time`),
  KEY `idx_accid_time` (`accId`, `time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `giftcodes`
--

CREATE TABLE IF NOT EXISTS `giftcodes` (
  `code` VARCHAR(128) NOT NULL,
  `content` JSON NOT NULL,
  `accId` BIGINT UNSIGNED NOT NULL DEFAULT '0',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `redeemed_at` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`code`),
  KEY `idx_accid` (`accId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `globalnews`
--

CREATE TABLE IF NOT EXISTS `globalnews` (
  `slot` SMALLINT UNSIGNED NOT NULL,
  `linkType` TINYINT UNSIGNED NOT NULL,
  `title` VARCHAR(128) NOT NULL,
  `image` TEXT NOT NULL,
  `priority` TINYINT UNSIGNED NOT NULL DEFAULT '0',
  `linkDetail` TEXT NOT NULL,
  `platform` VARCHAR(128) NOT NULL DEFAULT 'kabam.com,kongregate,steam,rotmg',
  `startTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `endTime` DATETIME NOT NULL,
  PRIMARY KEY (`slot`),
  KEY `idx_time_range` (`startTime`, `endTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `guilds`
--

CREATE TABLE IF NOT EXISTS `guilds` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(128) NOT NULL DEFAULT 'DEFAULT_GUILD',
  `members` TEXT NOT NULL,
  `guildFame` INT UNSIGNED NOT NULL DEFAULT '0',
  `totalGuildFame` INT UNSIGNED NOT NULL DEFAULT '0',
  `level` TINYINT UNSIGNED NOT NULL DEFAULT '1',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`),
  KEY `idx_level` (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `mysteryboxes`
--

CREATE TABLE IF NOT EXISTS `mysteryboxes` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `title` VARCHAR(128) NOT NULL,
  `weight` SMALLINT UNSIGNED NOT NULL,
  `description` VARCHAR(256) NOT NULL,
  `contents` JSON NOT NULL,
  `priceAmount` INT UNSIGNED NOT NULL,
  `priceCurrency` TINYINT UNSIGNED NOT NULL,
  `image` TEXT NOT NULL,
  `icon` TEXT NOT NULL,
  `salePrice` INT UNSIGNED NOT NULL DEFAULT '0',
  `saleCurrency` TINYINT UNSIGNED NOT NULL DEFAULT '0',
  `saleEnd` DATETIME NULL DEFAULT NULL,
  `startTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `boxEnd` DATETIME NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_time_range` (`startTime`, `boxEnd`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `news`
--

CREATE TABLE IF NOT EXISTS `news` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `icon` VARCHAR(16) NOT NULL DEFAULT 'info',
  `title` VARCHAR(128) NOT NULL DEFAULT 'Default news title',
  `text` TEXT NOT NULL,
  `link` VARCHAR(512) NOT NULL DEFAULT 'http://mmoe.net/',
  `date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_date` (`date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `packages`
--

CREATE TABLE IF NOT EXISTS `packages` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(128) NOT NULL,
  `maxPurchase` SMALLINT NOT NULL DEFAULT '-1',
  `weight` SMALLINT UNSIGNED NOT NULL DEFAULT '0',
  `contents` JSON NOT NULL,
  `bgUrl` VARCHAR(512) NOT NULL,
  `price` INT UNSIGNED NOT NULL,
  `quantity` INT NOT NULL DEFAULT '-1',
  `endDate` DATETIME NULL DEFAULT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_enddate` (`endDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `pets`
--

CREATE TABLE IF NOT EXISTS `pets` (
  `accId` BIGINT UNSIGNED NOT NULL,
  `petId` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `objType` SMALLINT UNSIGNED NOT NULL,
  `skinName` VARCHAR(128) NOT NULL,
  `skin` SMALLINT UNSIGNED NOT NULL,
  `family` TINYINT UNSIGNED NOT NULL,
  `rarity` TINYINT UNSIGNED NOT NULL,
  `maxLevel` TINYINT UNSIGNED NOT NULL DEFAULT '30',
  `abilities` JSON NOT NULL,
  `levels` JSON NOT NULL,
  `xp` JSON NOT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`petId`),
  KEY `idx_accid` (`accId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `stats`
--

CREATE TABLE IF NOT EXISTS `stats` (
  `accId` BIGINT UNSIGNED NOT NULL,
  `fame` INT UNSIGNED NOT NULL DEFAULT '0',
  `totalFame` INT UNSIGNED NOT NULL DEFAULT '0',
  `credits` INT UNSIGNED NOT NULL DEFAULT '0',
  `totalCredits` INT UNSIGNED NOT NULL DEFAULT '0',
  `fortuneTokens` INT UNSIGNED NOT NULL DEFAULT '0',
  `totalFortuneTokens` INT UNSIGNED NOT NULL DEFAULT '0',
  PRIMARY KEY (`accId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `thealchemist`
--

CREATE TABLE IF NOT EXISTS `thealchemist` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `title` VARCHAR(512) NOT NULL,
  `description` TEXT,
  `image` VARCHAR(512) NOT NULL,
  `icon` VARCHAR(512) NOT NULL,
  `contents` JSON NOT NULL,
  `priceFirstInGold` SMALLINT UNSIGNED NOT NULL DEFAULT '51',
  `priceFirstInToken` SMALLINT UNSIGNED NOT NULL DEFAULT '1',
  `priceSecondInGold` SMALLINT UNSIGNED NOT NULL DEFAULT '75',
  `startTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `endTime` DATETIME NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_time_range` (`startTime`, `endTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `unlockedclasses`
--

CREATE TABLE IF NOT EXISTS `unlockedclasses` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `accId` BIGINT UNSIGNED NOT NULL,
  `class` VARCHAR(128) NOT NULL,
  `available` JSON NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_accid` (`accId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for `vaults`
--

CREATE TABLE IF NOT EXISTS `vaults` (
  `chestId` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `accId` BIGINT UNSIGNED NOT NULL,
  `items` JSON NOT NULL,
  PRIMARY KEY (`chestId`),
  KEY `idx_accid` (`accId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;