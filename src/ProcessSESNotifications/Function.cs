/**
 *
 * Copyright (c) 2017 Rodney S.K. Lai
 * https://github.com/rodney-lai
 *
 * Permission to use, copy, modify, and/or distribute this software for
 * any purpose with or without fee is hereby granted, provided that the
 * above copyright notice and this permission notice appear in all copies.
 *
 * THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
 * WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
 * ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
 * WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
 * ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
 * OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using Amazon.Lambda.SNSEvents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using log4net;
using log4net.Config;
using log4net.Repository;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ProcessSESNotifications
{
  public class Function {
    private static readonly ILoggerRepository m_logRepo = LogManager.CreateRepository("ProcessSESNotifications");
    private static readonly ILog m_log = LogManager.GetLogger("ProcessSESNotifications",typeof(Function));

    public async Task<string> FunctionHandler(SNSEvent snsEvent, ILambdaContext context) {
      string mongoConnectionString = Environment.GetEnvironmentVariable("MongoDB_ConnectionString");
      DateTime now = DateTime.UtcNow;

      XmlConfigurator.Configure(m_logRepo, new System.IO.FileInfo("log4net.config"));
      foreach (var record in snsEvent.Records) {
        var snsRecord = record.Sns;
        JObject message = JObject.Parse(snsRecord.Message);
        string notificationId;

        switch (message["notificationType"].ToString()) {
          case "Bounce":
            notificationId = message["bounce"]["feedbackId"].ToString();
            break;
          case "Complaint":
            notificationId = message["complaint"]["feedbackId"].ToString();
            break;
          default:
            notificationId = "";
            break;
        }

        m_log.Info($"[{record.EventSource} {snsRecord.Timestamp}] Message = {snsRecord.Message}");
        if (!String.IsNullOrEmpty(mongoConnectionString)) {
          string mongoDatabaseName = mongoConnectionString.Substring(mongoConnectionString.LastIndexOf("/") + 1);
          MongoClient mongoClient = new MongoClient(mongoConnectionString);
          IMongoDatabase mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);
          IMongoCollection<EmailNotificationLog> emailNotificationLogCollection = mongoDatabase.GetCollection<EmailNotificationLog>("EmailNotificationLog");
          IMongoCollection<EmailBlackList> emailBlackListCollection = mongoDatabase.GetCollection<EmailBlackList>("EmailBlackList");

          emailBlackListCollection.Indexes.CreateOne(new BsonDocument("EmailAddress",1),new CreateIndexOptions { Name = "UX_EmailBlackList_EmailAddress", Unique = true });
          await emailNotificationLogCollection.InsertOneAsync(new EmailNotificationLog {
            NotificationType = message["notificationType"].ToString(),
            NotificationId = notificationId,
            Event = BsonSerializer.Deserialize<BsonDocument>(snsRecord.Message),
            Timestamp = snsRecord.Timestamp,
            Created = now
          });
          if ((message["notificationType"].ToString() == "Bounce") && (message["bounce"]["bounceType"].ToString() == "Permanent")) {
            foreach (JObject bouncedRecipient in message["bounce"]["bouncedRecipients"]) {
              string emailAddress = bouncedRecipient["emailAddress"].ToString().ToLower();
              long count = await emailBlackListCollection.Find(Builders<EmailBlackList>.Filter.Where(x => x.EmailAddress == emailAddress)).CountAsync();

              if (count == 0) {
                await emailBlackListCollection.InsertOneAsync(new EmailBlackList {
                  EmailAddress = emailAddress,
                  Created = now
                });
              }
            }
          }
        }
        if (EmailDbContext.IsActive()) {
          using (var db = new EmailDbContext()) {
            var emailNotificationLog = new EmailNotificationLog {
              NotificationType = message["notificationType"].ToString(),
              NotificationId = notificationId,
              Message = snsRecord.Message,
              Timestamp = snsRecord.Timestamp,
              Created = now
            };

            db.EmailNotificationLog.Add(emailNotificationLog);
            if ((message["notificationType"].ToString() == "Bounce") && (message["bounce"]["bounceType"].ToString() == "Permanent")) {
              foreach (JObject bouncedRecipient in message["bounce"]["bouncedRecipients"]) {
                string emailAddress = bouncedRecipient["emailAddress"].ToString().ToLower();

                if (db.EmailBlackList.Where(x => x.EmailAddress == emailAddress).Count() == 0) {
                  var emailBlackList = new EmailBlackList {
                    EmailAddress = emailAddress,
                    Created = now
                  };

                  db.EmailBlackList.Add(emailBlackList);
                }
              }
            }
            db.SaveChanges();
          }
        }
      }
      return ("okay");
    }
  }
}
