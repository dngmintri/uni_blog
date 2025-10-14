-- seed.sql
-- Dùng cho schema hiện tại (bảng: users, posts, comments; cột PascalCase)

SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE comments;
TRUNCATE TABLE posts;
TRUNCATE TABLE users;
SET FOREIGN_KEY_CHECKS = 1;

-- Users (PasswordHash tạm thời đặt chuỗi bất kỳ; sau này bạn có thể đăng ký user mới qua API)
INSERT INTO users
    (UserId, Username, Email, FullName, PasswordHash, DateOfBirth, Gender, Role, CreatedAt, LastLogin, IsActive)
VALUES
    (1, 'admin', 'admin@example.com', 'Administrator', 'seeded-hash', NULL, NULL, 'Admin', NOW(6), NULL, 1),
    (2, 'john',  'john@example.com',  'John Doe',     'seeded-hash', '2002-01-01', 'Male', 'User', NOW(6), NULL, 1);

-- Posts
INSERT INTO posts
    (PostId, UserId, Title, Content, ImageUrl, CreatedAt, UpdatedAt, Views, IsPublished, IsDeleted)
VALUES
    (1, 1, 'Welcome to Uni Blog', 'Bài viết đầu tiên của admin.', NULL, NOW(6), NULL, 0, 1, 0),
    (2, 2, 'Hello Uni Blog',      'Xin chào mọi người!',         NULL, NOW(6), NULL, 0, 1, 0);

-- Comments
INSERT INTO comments
    (CommentId, PostId, UserId, Content, CreatedAt, IsDeleted)
VALUES
    (1, 1, 2, 'Nice post!',         NOW(6), 0),
    (2, 1, 1, 'Cảm ơn bạn đã phản hồi.', NOW(6), 0);