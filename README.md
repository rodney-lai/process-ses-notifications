# AWS Lambda to process SES Notifications for Bounces and Complaints from SNS topics

[![Build Status](https://travis-ci.org/rodney-lai/process-ses-notifications.svg?branch=master)](https://travis-ci.org/rodney-lai/process-ses-notifications)

This AWS Lambda will log all SES notifications for bounces and complaints to either PostgreSQL or MongoDB and automatically put permanent bounces on an email black list.  

To setup:

1. Create SNS topics for bounces and complaints

2. Configure SES to publish bounce and complaint notifications to SNS topics

3. Restore Packages  
  `dotnet restore`

4. Package Lambda  
  ```
  cd "src/ProcessSESNotifications"
  ```  
  ```
  dotnet lambda package
  ```

5. Upload Lambda Package to AWS  

6. Configure Environment Variables  
*PostgreSQL_ConnectionString* = PostgreSQL connection string (optional)  
*PostgreSQL_Schema* = PostgresSQL schema (optional)  
*MongoDB_ConnectionString* = MongoDB connection string (optional)  

7. Add Triggers to Lambda from SNS topics

This AWS Lambda was created based on information in this blog post about handling bounces and complaints: https://aws.amazon.com/blogs/ses/handling-bounces-and-complaints/



Copyright (c) 2017 Rodney S.K. Lai

Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted, provided that the above copyright notice and this permission notice appear in all copies.  

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
