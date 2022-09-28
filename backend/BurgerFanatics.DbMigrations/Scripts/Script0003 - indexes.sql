-- we make sure to index our fks. This is just good habit, because of JOINs
create index idx_restaurant_administrator_id
on restaurant(administrator_id);

create index idx_restaurant_address_id
on restaurant(address_id);

create index idx_work_calendar_restaurant_id
on work_calendar(restaurant_id);

create index idx_review_user_id
on review(user_id);

create index idx_review_restaurant_id
on review(restaurant_id);

create index idx_file_attachment_review_id
on file_attachment(review_id);

-- special index provided by PostGIS
create index idx_address_location
on address using gist(location);