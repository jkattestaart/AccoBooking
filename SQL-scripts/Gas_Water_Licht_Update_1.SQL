﻿/*
Deployment script for Prod-AccoBooking

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "Prod-AccoBooking"
:setvar DefaultFilePrefix "Prod-AccoBooking"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Dropping FK_AccoAddition_Acco...';


GO
ALTER TABLE [dbo].[AccoAddition] DROP CONSTRAINT [FK_AccoAddition_Acco];


GO
PRINT N'Dropping FK_AccoAdditionDescription_AccoAddition...';


GO
ALTER TABLE [dbo].[AccoAdditionDescription] DROP CONSTRAINT [FK_AccoAdditionDescription_AccoAddition];


GO
PRINT N'Dropping FK_BookingAddition_AccoAddition...';


GO
ALTER TABLE [dbo].[BookingAddition] DROP CONSTRAINT [FK_BookingAddition_AccoAddition];


GO
PRINT N'Dropping FK_BookingPayment_Booking...';


GO
ALTER TABLE [dbo].[BookingPayment] DROP CONSTRAINT [FK_BookingPayment_Booking];


GO
PRINT N'Dropping FK_BookingCancelCondition_Booking...';


GO
ALTER TABLE [dbo].[BookingCancelCondition] DROP CONSTRAINT [FK_BookingCancelCondition_Booking];


GO
PRINT N'Dropping FK_BookingReminder_Booking...';


GO
ALTER TABLE [dbo].[BookingReminder] DROP CONSTRAINT [FK_BookingReminder_Booking];


GO
PRINT N'Dropping FK_Booking_Acco...';


GO
ALTER TABLE [dbo].[Booking] DROP CONSTRAINT [FK_Booking_Acco];


GO
PRINT N'Dropping FK_Booking_BookingGuest...';


GO
ALTER TABLE [dbo].[Booking] DROP CONSTRAINT [FK_Booking_BookingGuest];


GO
PRINT N'Dropping FK_BookingGuest_Booking...';


GO
ALTER TABLE [dbo].[BookingGuest] DROP CONSTRAINT [FK_BookingGuest_Booking];


GO
PRINT N'Dropping FK_BookingAddition_Booking...';


GO
ALTER TABLE [dbo].[BookingAddition] DROP CONSTRAINT [FK_BookingAddition_Booking];


GO
PRINT N'Starting rebuilding table [dbo].[AccoAddition]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_AccoAddition] (
    [AccoAdditionId]     INT            NOT NULL,
    [AccoId]             INT            NOT NULL,
    [DisplaySequence]    INT            NOT NULL,
    [Price]              DECIMAL (7, 2) NOT NULL,
    [Unit]               NVARCHAR (50)  NOT NULL,
    [IsDefaultBooked]    BIT            NOT NULL,
    [IsPayedFromDeposit] BIT            NULL,
    [Timestamp]          DATETIME       NULL,
    [Created]            DATETIME       NULL,
    [CreatedUser]        NVARCHAR (MAX) NULL,
    [Modified]           DATETIME       NULL,
    [ModifyUser]         NVARCHAR (MAX) NULL,
    [RowVersion]         INT            NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_AccoAddition] PRIMARY KEY CLUSTERED ([AccoAdditionId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[AccoAddition])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_AccoAddition] ([AccoAdditionId], [AccoId], [DisplaySequence], [Price], [Unit], [IsDefaultBooked], [Timestamp], [Created], [CreatedUser], [Modified], [ModifyUser], [RowVersion])
        SELECT   [AccoAdditionId],
                 [AccoId],
                 [DisplaySequence],
                 [Price],
                 [Unit],
                 [IsDefaultBooked],
                 [Timestamp],
                 [Created],
                 [CreatedUser],
                 [Modified],
                 [ModifyUser],
                 [RowVersion]
        FROM     [dbo].[AccoAddition]
        ORDER BY [AccoAdditionId] ASC;
    END

DROP TABLE [dbo].[AccoAddition];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_AccoAddition]', N'AccoAddition';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_AccoAddition]', N'PK_AccoAddition', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[AccoAddition].[IX_AccoAddition_Acco]...';


GO
CREATE NONCLUSTERED INDEX [IX_AccoAddition_Acco]
    ON [dbo].[AccoAddition]([AccoId] ASC);


GO
PRINT N'Creating [dbo].[AccoAddition].[IX_AccoAddition_DisplaySequence]...';


GO
CREATE NONCLUSTERED INDEX [IX_AccoAddition_DisplaySequence]
    ON [dbo].[AccoAddition]([AccoId] ASC, [DisplaySequence] ASC);


GO
PRINT N'Starting rebuilding table [dbo].[Booking]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Booking] (
    [BookingId]                          INT            NOT NULL,
    [AccoId]                             INT            NOT NULL,
    [GuestId]                            INT            NOT NULL,
    [Booked]                             DATETIME       NOT NULL,
    [Confirmed]                          DATETIME       NULL,
    [Arrival]                            DATETIME       NOT NULL,
    [Departure]                          DATETIME       NOT NULL,
    [Adults]                             INT            NOT NULL,
    [Children]                           INT            NOT NULL,
    [Pets]                               INT            NOT NULL,
    [Additions]                          DECIMAL (7, 2) NOT NULL,
    [Rent]                               DECIMAL (7, 2) NOT NULL,
    [Deposit]                            DECIMAL (7, 2) NOT NULL,
    [DaysToPayDepositBackAfterDeparture] INT            NOT NULL,
    [CancelAdministrationCosts]          DECIMAL (7, 2) NOT NULL,
    [Notes]                              NVARCHAR (MAX) NULL,
    [Status]                             NVARCHAR (50)  NOT NULL,
    [StatusUpdate]                       DATETIME       NOT NULL,
    [IsConfirmed]                        BIT            NOT NULL,
    [IsAmountExactlyScheduled]           BIT            NOT NULL,
    [ArrivalGas]                         INT            NULL,
    [ArrivalWater]                       INT            NULL,
    [ArrivalElectricity]                 INT            NULL,
    [DepartureGas]                       INT            NULL,
    [DepartureWater]                     INT            NULL,
    [DepartureElectricity]               INT            NULL,
    [Timestamp]                          DATETIME       NULL,
    [Created]                            DATETIME       NULL,
    [CreatedUser]                        NVARCHAR (MAX) NULL,
    [Modified]                           DATETIME       NULL,
    [ModifyUser]                         NVARCHAR (MAX) NULL,
    [RowVersion]                         INT            NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Booking] PRIMARY KEY CLUSTERED ([BookingId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Booking])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_Booking] ([BookingId], [AccoId], [GuestId], [Booked], [Confirmed], [Arrival], [Departure], [Adults], [Children], [Pets], [Additions], [Rent], [Deposit], [DaysToPayDepositBackAfterDeparture], [CancelAdministrationCosts], [Notes], [Status], [StatusUpdate], [IsConfirmed], [IsAmountExactlyScheduled], [Timestamp], [Created], [CreatedUser], [Modified], [ModifyUser], [RowVersion])
        SELECT   [BookingId],
                 [AccoId],
                 [GuestId],
                 [Booked],
                 [Confirmed],
                 [Arrival],
                 [Departure],
                 [Adults],
                 [Children],
                 [Pets],
                 [Additions],
                 [Rent],
                 [Deposit],
                 [DaysToPayDepositBackAfterDeparture],
                 [CancelAdministrationCosts],
                 [Notes],
                 [Status],
                 [StatusUpdate],
                 [IsConfirmed],
                 [IsAmountExactlyScheduled],
                 [Timestamp],
                 [Created],
                 [CreatedUser],
                 [Modified],
                 [ModifyUser],
                 [RowVersion]
        FROM     [dbo].[Booking]
        ORDER BY [BookingId] ASC;
    END

DROP TABLE [dbo].[Booking];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Booking]', N'Booking';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_Booking]', N'PK_Booking', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[Booking].[IX_Booking_Acco]...';


GO
CREATE NONCLUSTERED INDEX [IX_Booking_Acco]
    ON [dbo].[Booking]([AccoId] ASC);


GO
PRINT N'Creating [dbo].[Booking].[IX_Booking_Guest]...';


GO
CREATE NONCLUSTERED INDEX [IX_Booking_Guest]
    ON [dbo].[Booking]([GuestId] ASC);


GO
PRINT N'Creating [dbo].[Booking].[IX_Booking_Arrival]...';


GO
CREATE NONCLUSTERED INDEX [IX_Booking_Arrival]
    ON [dbo].[Booking]([Arrival] ASC);


GO
PRINT N'Creating [dbo].[Booking].[IX_Booking_Dparture]...';


GO
CREATE NONCLUSTERED INDEX [IX_Booking_Dparture]
    ON [dbo].[Booking]([Departure] ASC);


GO
PRINT N'Creating [dbo].[Booking].[IX_Booking_Status]...';


GO
CREATE NONCLUSTERED INDEX [IX_Booking_Status]
    ON [dbo].[Booking]([Status] ASC);


GO
PRINT N'Creating [dbo].[Booking].[IX_Booking_Booked]...';


GO
CREATE NONCLUSTERED INDEX [IX_Booking_Booked]
    ON [dbo].[Booking]([Booked] ASC);


GO
PRINT N'Starting rebuilding table [dbo].[BookingAddition]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BookingAddition] (
    [BookingAdditionId]  INT            NOT NULL,
    [BookingId]          INT            NOT NULL,
    [DisplaySequence]    INT            NOT NULL,
    [AccoAdditionId]     INT            NULL,
    [Description]        NVARCHAR (MAX) NULL,
    [Unit]               NVARCHAR (50)  NOT NULL,
    [Price]              DECIMAL (7, 2) NOT NULL,
    [IsPayedFromDeposit] BIT            NULL,
    [Timestamp]          DATETIME       NULL,
    [Created]            DATETIME       NULL,
    [CreatedUser]        NVARCHAR (MAX) NULL,
    [Modified]           DATETIME       NULL,
    [ModifyUser]         NVARCHAR (MAX) NULL,
    [RowVersion]         INT            NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_BookingAddition] PRIMARY KEY CLUSTERED ([BookingAdditionId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BookingAddition])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_BookingAddition] ([BookingAdditionId], [BookingId], [DisplaySequence], [AccoAdditionId], [Description], [Unit], [Price], [Timestamp], [Created], [CreatedUser], [Modified], [ModifyUser], [RowVersion])
        SELECT   [BookingAdditionId],
                 [BookingId],
                 [DisplaySequence],
                 [AccoAdditionId],
                 [Description],
                 [Unit],
                 [Price],
                 [Timestamp],
                 [Created],
                 [CreatedUser],
                 [Modified],
                 [ModifyUser],
                 [RowVersion]
        FROM     [dbo].[BookingAddition]
        ORDER BY [BookingAdditionId] ASC;
    END

DROP TABLE [dbo].[BookingAddition];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BookingAddition]', N'BookingAddition';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_BookingAddition]', N'PK_BookingAddition', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[BookingAddition].[IX_BookingAddition_Booking]...';


GO
CREATE NONCLUSTERED INDEX [IX_BookingAddition_Booking]
    ON [dbo].[BookingAddition]([BookingId] ASC);


GO
PRINT N'Creating [dbo].[BookingAddition].[IX_BookingAddition_AccoAddition]...';


GO
CREATE NONCLUSTERED INDEX [IX_BookingAddition_AccoAddition]
    ON [dbo].[BookingAddition]([AccoAdditionId] ASC);


GO
PRINT N'Creating [dbo].[BookingAddition].[IX_BookingAddition_DisplaySeqeuence]...';


GO
CREATE NONCLUSTERED INDEX [IX_BookingAddition_DisplaySeqeuence]
    ON [dbo].[BookingAddition]([BookingId] ASC, [DisplaySequence] ASC);


GO
PRINT N'Creating Full-text Index on [dbo].[Booking]...';


GO
CREATE FULLTEXT INDEX ON [dbo].[Booking]
    ([Notes] LANGUAGE 0)
    KEY INDEX [PK_Booking]
    ON [Text];


GO
PRINT N'Creating FK_AccoAddition_Acco...';


GO
ALTER TABLE [dbo].[AccoAddition] WITH NOCHECK
    ADD CONSTRAINT [FK_AccoAddition_Acco] FOREIGN KEY ([AccoId]) REFERENCES [dbo].[Acco] ([AccoId]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_AccoAdditionDescription_AccoAddition...';


GO
ALTER TABLE [dbo].[AccoAdditionDescription] WITH NOCHECK
    ADD CONSTRAINT [FK_AccoAdditionDescription_AccoAddition] FOREIGN KEY ([AccoAdditionId]) REFERENCES [dbo].[AccoAddition] ([AccoAdditionId]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BookingAddition_AccoAddition...';


GO
ALTER TABLE [dbo].[BookingAddition] WITH NOCHECK
    ADD CONSTRAINT [FK_BookingAddition_AccoAddition] FOREIGN KEY ([AccoAdditionId]) REFERENCES [dbo].[AccoAddition] ([AccoAdditionId]);


GO
PRINT N'Creating FK_BookingPayment_Booking...';


GO
ALTER TABLE [dbo].[BookingPayment] WITH NOCHECK
    ADD CONSTRAINT [FK_BookingPayment_Booking] FOREIGN KEY ([BookingId]) REFERENCES [dbo].[Booking] ([BookingId]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BookingCancelCondition_Booking...';


GO
ALTER TABLE [dbo].[BookingCancelCondition] WITH NOCHECK
    ADD CONSTRAINT [FK_BookingCancelCondition_Booking] FOREIGN KEY ([BookingId]) REFERENCES [dbo].[Booking] ([BookingId]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BookingReminder_Booking...';


GO
ALTER TABLE [dbo].[BookingReminder] WITH NOCHECK
    ADD CONSTRAINT [FK_BookingReminder_Booking] FOREIGN KEY ([BookingId]) REFERENCES [dbo].[Booking] ([BookingId]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_Booking_Acco...';


GO
ALTER TABLE [dbo].[Booking] WITH NOCHECK
    ADD CONSTRAINT [FK_Booking_Acco] FOREIGN KEY ([AccoId]) REFERENCES [dbo].[Acco] ([AccoId]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_Booking_BookingGuest...';


GO
ALTER TABLE [dbo].[Booking] WITH NOCHECK
    ADD CONSTRAINT [FK_Booking_BookingGuest] FOREIGN KEY ([GuestId]) REFERENCES [dbo].[BookingGuest] ([BookingGuestId]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BookingGuest_Booking...';


GO
ALTER TABLE [dbo].[BookingGuest] WITH NOCHECK
    ADD CONSTRAINT [FK_BookingGuest_Booking] FOREIGN KEY ([BookingId]) REFERENCES [dbo].[Booking] ([BookingId]);


GO
PRINT N'Creating FK_BookingAddition_Booking...';


GO
ALTER TABLE [dbo].[BookingAddition] WITH NOCHECK
    ADD CONSTRAINT [FK_BookingAddition_Booking] FOREIGN KEY ([BookingId]) REFERENCES [dbo].[Booking] ([BookingId]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[Booking].[Status].[MS_Description]...';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Reserved, Confirmed, Paid, Closed, Expired, Cancelled', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Booking', @level2type = N'COLUMN', @level2name = N'Status';


GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[AccoAddition] WITH CHECK CHECK CONSTRAINT [FK_AccoAddition_Acco];

ALTER TABLE [dbo].[AccoAdditionDescription] WITH CHECK CHECK CONSTRAINT [FK_AccoAdditionDescription_AccoAddition];

ALTER TABLE [dbo].[BookingAddition] WITH CHECK CHECK CONSTRAINT [FK_BookingAddition_AccoAddition];

ALTER TABLE [dbo].[BookingPayment] WITH CHECK CHECK CONSTRAINT [FK_BookingPayment_Booking];

ALTER TABLE [dbo].[BookingCancelCondition] WITH CHECK CHECK CONSTRAINT [FK_BookingCancelCondition_Booking];

ALTER TABLE [dbo].[BookingReminder] WITH CHECK CHECK CONSTRAINT [FK_BookingReminder_Booking];

ALTER TABLE [dbo].[Booking] WITH CHECK CHECK CONSTRAINT [FK_Booking_Acco];

ALTER TABLE [dbo].[Booking] WITH CHECK CHECK CONSTRAINT [FK_Booking_BookingGuest];

ALTER TABLE [dbo].[BookingGuest] WITH CHECK CHECK CONSTRAINT [FK_BookingGuest_Booking];

ALTER TABLE [dbo].[BookingAddition] WITH CHECK CHECK CONSTRAINT [FK_BookingAddition_Booking];


GO
PRINT N'Update complete.';


GO
