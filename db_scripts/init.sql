create database burger_fanatics_db;
create user burger_fanatics_db_user with password 'example' superuser;
grant all privileges on database burger_fanatics_db to burger_fanatics_db_user;