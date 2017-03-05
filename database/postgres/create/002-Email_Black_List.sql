CREATE TABLE IF NOT EXISTS "Email_Black_List" (
  "Email_Black_List_Id" BIGSERIAL,
  "Email_Address" VARCHAR(200) NOT NULL,
  "Created" TIMESTAMP WITH TIME ZONE DEFAULT now() NOT NULL,
  CONSTRAINT "PK_Email_Black_List" PRIMARY KEY ("Email_Black_List_Id")
);
CREATE UNIQUE INDEX IF NOT EXISTS "UX_Email_Black_List_Email_Address" ON "Email_Black_List" (LOWER("Email_Address"));
