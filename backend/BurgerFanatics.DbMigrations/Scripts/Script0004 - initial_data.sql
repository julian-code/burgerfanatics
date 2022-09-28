begin;
-- this is just dummy data to have something to show.
insert into "user" (user_id, username)
values ('C75CEB3C-83EF-4605-B9B3-77993B3CC383', 'jmathiaskock@gmail.com');

insert into "user" (user_id, username)
values ('3C8BE86F-559C-4F8F-9134-9E97C3BF2516', 'reviewer@gmail.com');

insert into address (address_id, text, location)
values ('0a3f50b3-ebcc-32b8-e044-0003ba298018', 'Gravene 2A, 5000 Odense C', 'POINT(10.38854476 55.39781416)');

insert into restaurant (restaurant_id, name, description, administrator_id, address_id)
values ('4F3BF172-42BA-48F9-AC2A-76178793638F', 'Burger Anarchy', 'I 2014 fik burgeren en renæssance i Odense, og det var os der startede den! Hos Burger Anarchy får du anarkistiske gourmet burgers. Vi bryder med konventionerne og siger skrid til traditionerne! Forvent nytænkende koncepter og flabede smagskombinationer', 'C75CEB3C-83EF-4605-B9B3-77993B3CC383', '0a3f50b3-ebcc-32b8-e044-0003ba298018');

insert into work_calendar (week_day, opening_hour, closing_hour, restaurant_id) values ('Monday', '12:00:00', '21:00:00', '4F3BF172-42BA-48F9-AC2A-76178793638F');
insert into work_calendar (week_day, opening_hour, closing_hour, restaurant_id) values ('Tuesday', '12:00:00', '21:00:00', '4F3BF172-42BA-48F9-AC2A-76178793638F');
insert into work_calendar (week_day, opening_hour, closing_hour, restaurant_id) values ('Thursday', '12:00:00', '21:00:00', '4F3BF172-42BA-48F9-AC2A-76178793638F');
insert into work_calendar (week_day, opening_hour, closing_hour, restaurant_id) values ('Friday', '12:00:00', '22:00:00', '4F3BF172-42BA-48F9-AC2A-76178793638F');
insert into work_calendar (week_day, opening_hour, closing_hour, restaurant_id) values ('Saturday', '12:00:00', '22:00:00', '4F3BF172-42BA-48F9-AC2A-76178793638F');
insert into work_calendar (week_day, opening_hour, closing_hour, restaurant_id) values ('Sunday', '12:00:00', '21:00:00', '4F3BF172-42BA-48F9-AC2A-76178793638F');

insert into review (review_id, description, rating_taste, rating_visual, rating_texture, created_utc, updated_utc, user_id, restaurant_id) 
values ('94EAE17E-41E1-4EB0-A67C-FAAD3FCFB2E8', 'A really nice experience with a lot of flavour, and nice burgers. Will recommend!', 8, 2, 5, now(), now(), '3C8BE86F-559C-4F8F-9134-9E97C3BF2516', '4F3BF172-42BA-48F9-AC2A-76178793638F');

commit;