select * from [User]

select * from Game

ALTER TABLE Game
ADD
    ImageUrl NVARCHAR(1000) NULL,
    ImagePath NVARCHAR(255) NULL;

UPDATE Game
SET ImageUrl = 'https://i0.wp.com/highschool.latimes.com/wp-content/uploads/2021/09/league-of-legends.jpeg?fit=1607%2C895&ssl=1'
WHERE Id = 2;
