﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TechExtensions.SchedyBot.DLL;

#nullable disable

namespace TechExtensions.SchedyBot.DLL.Migrations
{
    [DbContext(typeof(SchedulyBotContext))]
    [Migration("20220609084025_UpToDate")]
    partial class UpToDate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.ActionHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Action")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int?>("SubjectClientId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SubjectClientId");

                    b.ToTable("ActionsHistory");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<int>("Country")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FullAddress")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("BookTimeFrom")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("BookTimeTo")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("BookType")
                        .HasColumnType("integer");

                    b.Property<int?>("ClientId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("ExecutorId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("MessageForClient")
                        .HasColumnType("text");

                    b.Property<string>("MessageForExecutor")
                        .HasColumnType("text");

                    b.Property<int?>("ServiceAndSpecId")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("ExecutorId");

                    b.HasIndex("ServiceAndSpecId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.BookingInConfirmation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("BookingToConfirmId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("MessageForClient")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("TimeIsBeingChangedFrom")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("TimeIsBeingChangedTo")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("BookingToConfirmId");

                    b.ToTable("BookingInConfirmations");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AddressId")
                        .HasColumnType("integer");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<int?>("ContactId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSeller")
                        .HasColumnType("boolean");

                    b.Property<string>("Language")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Nickname")
                        .HasColumnType("text");

                    b.Property<TimeSpan>("TimeZoneOffset")
                        .HasColumnType("interval");

                    b.Property<int[]>("UsedSellerIds")
                        .HasColumnType("integer[]");

                    b.Property<bool>("WentThroughFullRegistration")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("ContactId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Contact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.CurrentDialog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CurrentBranchId")
                        .HasColumnType("integer");

                    b.Property<int>("CurrentStepId")
                        .HasColumnType("integer");

                    b.Property<int>("CurrentTemplateId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("CurrentDialogs");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.CurrentDialogIteration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BranchId")
                        .HasColumnType("integer");

                    b.Property<int>("CurrentDialogId")
                        .HasColumnType("integer");

                    b.Property<int>("StepId")
                        .HasColumnType("integer");

                    b.Property<int>("TemplateId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CurrentDialogId");

                    b.ToTable("CurrentDialogIterations");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int>("Rate")
                        .HasColumnType("integer");

                    b.Property<string>("ReviewText")
                        .HasColumnType("text");

                    b.Property<int?>("ReviewUserId")
                        .HasColumnType("integer");

                    b.Property<int?>("TargetUserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ReviewUserId");

                    b.HasIndex("TargetUserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Schedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("BreakTimeFrom")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("BreakTimeTo")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("TimeFrom")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("TimeTo")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("WeekDay")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsMain")
                        .HasColumnType("boolean");

                    b.Property<string>("LanguageCode")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.ServiceAndSpec", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int>("Price")
                        .HasColumnType("integer");

                    b.Property<int?>("ServiceId")
                        .HasColumnType("integer");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.HasIndex("UserId");

                    b.ToTable("ServiceAndSpecs");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Step", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BranchId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("CurrentDialogId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int>("StepId")
                        .HasColumnType("integer");

                    b.Property<int>("TemplateId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CurrentDialogId");

                    b.ToTable("Steps");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.UpdateMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ChatId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("HashCode")
                        .HasColumnType("integer");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("UpdateMessages");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.ActionHistory", b =>
                {
                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.Client", "SubjectClient")
                        .WithMany("ActionHistory")
                        .HasForeignKey("SubjectClientId");

                    b.Navigation("SubjectClient");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Booking", b =>
                {
                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId");

                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.Client", "Executor")
                        .WithMany()
                        .HasForeignKey("ExecutorId");

                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.ServiceAndSpec", "ServiceAndSpec")
                        .WithMany()
                        .HasForeignKey("ServiceAndSpecId");

                    b.Navigation("Client");

                    b.Navigation("Executor");

                    b.Navigation("ServiceAndSpec");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.BookingInConfirmation", b =>
                {
                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.Booking", "BookingToConfirm")
                        .WithMany()
                        .HasForeignKey("BookingToConfirmId");

                    b.Navigation("BookingToConfirm");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Client", b =>
                {
                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.Contact", "Contact")
                        .WithMany()
                        .HasForeignKey("ContactId");

                    b.Navigation("Address");

                    b.Navigation("Contact");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.CurrentDialogIteration", b =>
                {
                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.CurrentDialog", "CurrentDialog")
                        .WithMany("CurrentDialogIterations")
                        .HasForeignKey("CurrentDialogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CurrentDialog");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Review", b =>
                {
                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.Client", "ReviewUser")
                        .WithMany()
                        .HasForeignKey("ReviewUserId");

                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.Client", "TargetUser")
                        .WithMany()
                        .HasForeignKey("TargetUserId");

                    b.Navigation("ReviewUser");

                    b.Navigation("TargetUser");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Schedule", b =>
                {
                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.Client", null)
                        .WithMany("Schedules")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.ServiceAndSpec", b =>
                {
                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId");

                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.Client", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Service");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Step", b =>
                {
                    b.HasOne("TechExtensions.SchedyBot.DLL.Entities.CurrentDialog", null)
                        .WithMany("StepRoute")
                        .HasForeignKey("CurrentDialogId");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.Client", b =>
                {
                    b.Navigation("ActionHistory");

                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("TechExtensions.SchedyBot.DLL.Entities.CurrentDialog", b =>
                {
                    b.Navigation("CurrentDialogIterations");

                    b.Navigation("StepRoute");
                });
#pragma warning restore 612, 618
        }
    }
}
