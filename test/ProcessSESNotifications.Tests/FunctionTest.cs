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

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.SNSEvents;

using ProcessSESNotifications;

namespace ProcessSESNotifications.Tests
{
  public class FunctionTest {

    [Fact]
    public void TestFunction() {
      var function = new Function();
      var context = new TestLambdaContext();
      SNSEvent.SNSMessage snsMessage = new SNSEvent.SNSMessage {
        Message = "{\"notificationType\":\"Bounce\",\"bounce\":{\"bounceType\":\"Permanent\",\"feedbackId\":\"feedback_id\",\"bouncedRecipients\":[{\"emailAddress\":\"bounce@example.com\"}]}}",
        Timestamp = DateTime.Parse("1/1/1990")
      };
      SNSEvent.SNSRecord snsRecord = new SNSEvent.SNSRecord {
        EventSource = "event_source",
        EventSubscriptionArn = "event_subscription_arn",
        EventVersion = "event_version",
        Sns = snsMessage
      };
      List<SNSEvent.SNSRecord> recordList = new List<SNSEvent.SNSRecord>();
      recordList.Add(snsRecord);
      SNSEvent snsEvent = new SNSEvent {
        Records = recordList
      };

      var retval = function.FunctionHandler(snsEvent,context);

      Assert.Equal("okay", retval.Result);
    }

  }
}
