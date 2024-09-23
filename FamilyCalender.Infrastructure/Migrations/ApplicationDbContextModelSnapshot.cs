﻿// <auto-generated />
using System;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FamilyCalender.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("FamilyCalender.Core.Models.Calendar", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("OwnerId1")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId1");

                    b.ToTable("Calendars");
                });

            modelBuilder.Entity("FamilyCalender.Core.Models.CalendarAccess", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int?>("CalendarId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("CalendarId1")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsOwner")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("UserId1")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CalendarId1");

                    b.HasIndex("UserId1");

                    b.ToTable("CalendarAccesses");
                });

            modelBuilder.Entity("FamilyCalender.Core.Models.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("End")
                        .HasColumnType("TEXT");

                    b.Property<int?>("MemberId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("MemberId1")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Start")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("MemberId1");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("FamilyCalender.Core.Models.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int?>("CalendarId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("CalendarId1")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CalendarId1");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("FamilyCalender.Core.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FamilyCalender.Core.Models.Calendar", b =>
                {
                    b.HasOne("FamilyCalender.Core.Models.User", "Owner")
                        .WithMany("OwnedCalendars")
                        .HasForeignKey("OwnerId1");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("FamilyCalender.Core.Models.CalendarAccess", b =>
                {
                    b.HasOne("FamilyCalender.Core.Models.Calendar", "Calendar")
                        .WithMany()
                        .HasForeignKey("CalendarId1");

                    b.HasOne("FamilyCalender.Core.Models.User", "User")
                        .WithMany("CalendarAccesses")
                        .HasForeignKey("UserId1");

                    b.Navigation("Calendar");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FamilyCalender.Core.Models.Event", b =>
                {
                    b.HasOne("FamilyCalender.Core.Models.Member", "Member")
                        .WithMany("Events")
                        .HasForeignKey("MemberId1");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("FamilyCalender.Core.Models.Member", b =>
                {
                    b.HasOne("FamilyCalender.Core.Models.Calendar", "Calendar")
                        .WithMany("Members")
                        .HasForeignKey("CalendarId1");

                    b.Navigation("Calendar");
                });

            modelBuilder.Entity("FamilyCalender.Core.Models.Calendar", b =>
                {
                    b.Navigation("Members");
                });

            modelBuilder.Entity("FamilyCalender.Core.Models.Member", b =>
                {
                    b.Navigation("Events");
                });

            modelBuilder.Entity("FamilyCalender.Core.Models.User", b =>
                {
                    b.Navigation("CalendarAccesses");

                    b.Navigation("OwnedCalendars");
                });
#pragma warning restore 612, 618
        }
    }
}
