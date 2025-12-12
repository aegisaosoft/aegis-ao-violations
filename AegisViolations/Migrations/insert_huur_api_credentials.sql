-- Insert Huur API authentication credentials into settings table
-- These credentials will be used to authenticate with the Huur API

-- Insert or update Huur API email (uid)
-- First, delete if exists, then insert
DELETE FROM settings WHERE key = 'huur.api.email';
INSERT INTO settings (key, value, created_at, updated_at)
VALUES ('huur.api.email', 'mirzoev.siyovush@outlook.com', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Insert or update Huur API password
-- First, delete if exists, then insert
DELETE FROM settings WHERE key = 'huur.api.password';
INSERT INTO settings (key, value, created_at, updated_at)
VALUES ('huur.api.password', 'Iroc@2020', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

