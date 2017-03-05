CREATE TABLE IF NOT EXISTS "Email_Notification_Log" (
  "Email_Notification_Log_Id" BIGSERIAL,
  "Notification_Type" VARCHAR(20) NOT NULL,
  "Notification_Id" VARCHAR(80) NOT NULL,
  "Message" JSONB NOT NULL,
  "Timestamp" TIMESTAMP WITH TIME ZONE NOT NULL,
  "Created" TIMESTAMP WITH TIME ZONE DEFAULT now() NOT NULL,
  CONSTRAINT "PK_Email_Notification_Log" PRIMARY KEY ("Email_Notification_Log_Id")
);
