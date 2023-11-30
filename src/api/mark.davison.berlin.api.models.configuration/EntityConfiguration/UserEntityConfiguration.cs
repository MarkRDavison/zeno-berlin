﻿namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .ValueGeneratedNever();

        builder
            .Property(_ => _.Sub);

        builder
            .Property(_ => _.Username)
            .HasMaxLength(64);

        builder
            .Property(_ => _.First)
            .HasMaxLength(255);

        builder
            .Property(_ => _.Last)
            .HasMaxLength(62554);

        builder
            .Property(_ => _.Email)
            .HasMaxLength(255);

        builder
            .Property(_ => _.Admin);
    }
}