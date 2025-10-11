-- ==========================
-- Table: users
-- ==========================
CREATE TABLE `users` (
  `user_id` INT NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(50) COLLATE utf8mb4_unicode_ci NOT NULL UNIQUE,
  `password_hash` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` VARCHAR(100) COLLATE utf8mb4_unicode_ci NOT NULL UNIQUE,
  `full_name` VARCHAR(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `date_of_birth` DATE DEFAULT NULL,
  `gender` VARCHAR(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL, -- Nam/Nu/Khac, flexible
  `role` ENUM('User','Admin') COLLATE utf8mb4_unicode_ci DEFAULT 'User',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `last_login` DATETIME DEFAULT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==========================
-- Table: posts
-- ==========================
CREATE TABLE `posts` (
  `post_id` INT NOT NULL AUTO_INCREMENT,
  `user_id` INT NOT NULL,
  `title` VARCHAR(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `content` TEXT COLLATE utf8mb4_unicode_ci NOT NULL,
  `image_url` VARCHAR(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `views` INT DEFAULT 0,
  `is_published` TINYINT(1) DEFAULT 1,
  `is_deleted` TINYINT(1) DEFAULT 0,
  PRIMARY KEY (`post_id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `posts_user_fk` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==========================
-- Table: comments
-- ==========================
CREATE TABLE `comments` (
  `comment_id` INT NOT NULL AUTO_INCREMENT,
  `post_id` INT NOT NULL,
  `user_id` INT NOT NULL,
  `content` TEXT COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `is_deleted` TINYINT(1) DEFAULT 0,
  PRIMARY KEY (`comment_id`),
  KEY `post_id` (`post_id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `comments_post_fk` FOREIGN KEY (`post_id`) REFERENCES `posts` (`post_id`) ON DELETE CASCADE,
  CONSTRAINT `comments_user_fk` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

