
DELETE FROM Health;
INSERT INTO Health (Msg) VALUES ('Hello world from SQL Server DB!');

DELETE FROM users;
INSERT INTO users (password_hash, login, full_name) VALUES
  ('','shelly','Sheldon'),
  ('','lenny','Leonard'),
  ('','raj','Rajesh'),
  ('','howie','Howard');



