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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProcessSESNotifications {

  public partial class EmailDbContext : DbContext {

    private static string m_connectionString = Environment.GetEnvironmentVariable("PostgreSQL_ConnectionString");

    public static bool IsActive() {
        return(!String.IsNullOrEmpty(m_connectionString));
    }

    public virtual DbSet<EmailBlackList> EmailBlackList { get; set; }
    public virtual DbSet<EmailNotificationLog> EmailNotificationLog { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      optionsBuilder.UseNpgsql(m_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      string schema = Environment.GetEnvironmentVariable("PostgreSQL_Schema");

      if (!String.IsNullOrWhiteSpace(schema)) modelBuilder.HasDefaultSchema(schema);

      modelBuilder.Entity<EmailBlackList>(entity =>
      {
        entity.ToTable("Email_Black_List");

        entity.Property(e => e.EmailBlackListId).HasColumnName("Email_Black_List_Id");

        entity.Property(e => e.Created)
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()");

        entity.Property(e => e.EmailAddress)
            .IsRequired()
            .HasColumnName("Email_Address")
            .HasColumnType("varchar")
            .HasMaxLength(200);
      });

      modelBuilder.Entity<EmailNotificationLog>(entity =>
      {
        entity.ToTable("Email_Notification_Log");

        entity.Property(e => e.EmailNotificationLogId).HasColumnName("Email_Notification_Log_Id");

        entity.Property(e => e.Created)
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()");

        entity.Property(e => e.Message)
            .IsRequired()
            .HasColumnType("jsonb");

        entity.Property(e => e.NotificationId)
            .IsRequired()
            .HasColumnName("Notification_Id")
            .HasColumnType("varchar")
            .HasMaxLength(80);

        entity.Property(e => e.NotificationType)
            .IsRequired()
            .HasColumnName("Notification_Type")
            .HasColumnType("varchar")
            .HasMaxLength(20);

        entity.Property(e => e.Timestamp).HasColumnType("timestamptz");
      });
    }
  }

}
