alter table restaurant
add constraint fk_restaurant_user
foreign key (administrator_id)
references "user" (user_id) 
on delete set null;

alter table restaurant
add constraint fk_restaurant_address
foreign key (address_id)
references address (address_id)
on delete restrict;

alter table review
add constraint fk_review_user
foreign key (user_id)
references "user" (user_id)
on delete set null;

alter table review
add constraint fk_review_restaurant
foreign key (restaurant_id)
references restaurant(restaurant_id) 
on delete cascade;

alter table work_calendar
add constraint fk_work_calendar_restaurant
foreign key (restaurant_id)
references restaurant(restaurant_id)
on delete cascade;

alter table file_attachment
add constraint fk_file_attachment_review
foreign key (review_id)
references review(review_id)
on delete cascade;