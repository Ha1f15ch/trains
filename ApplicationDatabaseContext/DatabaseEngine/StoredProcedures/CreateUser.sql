CREATE OR REPLACE PROCEDURE insertuserdata(
    name TEXT,
    email TEXT,
    password TEXT,
    is_active BOOLEAN,
    date_create DATE,
    date_update DATE,
    date_delete DATE
)
LANGUAGE plpgsql AS
$$
BEGIN
    INSERT INTO dbo."User" ("Name", "Email", "Password", "UserIsActive", "DateCreate", "DateUpdate", "DateDelete")
    VALUES (name, email, password, is_active, date_create, date_update, date_delete);
END;
$$;


