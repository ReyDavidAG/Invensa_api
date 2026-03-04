USE [master]
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'Invensa')
BEGIN
    ALTER DATABASE [Invensa] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [Invensa];
END
GO

CREATE DATABASE [Invensa]
GO

USE [Invensa]
GO

CREATE TABLE [dbo].[Unit](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](16) NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[CreatedAtUtc] [datetime2](3) NOT NULL,
	[UpdatedAtUtc] [datetime2](3) NULL,
	[RowVersion] [timestamp] NOT NULL,
	CONSTRAINT [PK_Unit] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UQ_Unit_Code] UNIQUE NONCLUSTERED ([Code] ASC)
);
GO

CREATE TABLE [dbo].[ImageFiles](
	[Id] [uniqueidentifier] NOT NULL,
	[OriginalFileName] [nvarchar](256) NOT NULL,
	[ContentType] [nvarchar](128) NOT NULL,
	[SizeBytes] [bigint] NOT NULL,
	[Sha256] [nvarchar](64) NOT NULL,
	[StoragePath] [nvarchar](512) NOT NULL,
	[CreatedAtUtc] [datetime2](7) NOT NULL,
	CONSTRAINT [PK_ImageFiles] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[User](
	[Id] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[PasswordHash] [nvarchar](256) NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[Active] [bit] NOT NULL,
	[CreatedAtUtc] [datetime2](3) NOT NULL,
	[UpdatedAtUtc] [datetime2](3) NULL,
	[RowVersion] [timestamp] NOT NULL,
	CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UQ_User_Email] UNIQUE NONCLUSTERED ([Email] ASC)
);
GO

CREATE TABLE [dbo].[Sale](
	[Id] [uniqueidentifier] NOT NULL,
	[DateUtc] [datetime2](3) NOT NULL,
	[Total] [decimal](18, 2) NOT NULL,
	[TicketNumber] [bigint] IDENTITY(1,1) NOT NULL,
	[CreatedAtUtc] [datetime2](3) NOT NULL,
	[UpdatedAtUtc] [datetime2](3) NULL,
	[RowVer] [timestamp] NOT NULL,
	CONSTRAINT [PK_Sale] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[Product](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](120) NOT NULL,
	[Code] [nvarchar](64) NOT NULL,
	[UnitId] [uniqueidentifier] NOT NULL,
	[PriceSale] [decimal](18, 2) NOT NULL,
	[PriceBuy] [decimal](18, 2) NOT NULL,
	[ImageId] [uniqueidentifier] NULL,
	[CreatedAtUtc] [datetime2](3) NOT NULL,
	[UpdatedAtUtc] [datetime2](3) NULL,
	[RowVersion] [timestamp] NOT NULL,
	CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UQ_Product_Code] UNIQUE NONCLUSTERED ([Code] ASC)
);
GO

CREATE TABLE [dbo].[RefreshToken](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Token] [nvarchar](256) NOT NULL,
	[ExpiresAtUtc] [datetime2](3) NOT NULL,
	[CreatedAtUtc] [datetime2](3) NOT NULL,
	[RevokedAtUtc] [datetime2](3) NULL,
	[IsRevoked] [bit] NOT NULL,
	CONSTRAINT [PK_RefreshToken] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UQ_RefreshToken_Token] UNIQUE NONCLUSTERED ([Token] ASC)
);
GO

CREATE TABLE [dbo].[SaleItem](
	[Id] [uniqueidentifier] NOT NULL,
	[SaleId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Quantity] [decimal](18, 4) NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[Subtotal] AS (CONVERT([decimal](18,2),[Quantity]*[UnitPrice])) PERSISTED,
	[CreatedAtUtc] [datetime2](3) NOT NULL,
	[RowVer] [timestamp] NOT NULL,
	CONSTRAINT [PK_SaleItem] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[InventoryMovement](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[MovementType] [tinyint] NOT NULL,
	[Quantity] [decimal](18, 4) NULL,
	[QuantityAdj] [decimal](18, 4) NULL,
	[UnitPrice] [decimal](18, 2) NULL,
	[SaleId] [uniqueidentifier] NULL,
	[Note] [nvarchar](200) NULL,
	[CreatedAtUtc] [datetime2](3) NOT NULL,
	CONSTRAINT [PK_InventoryMovement] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_ImageFiles_StoragePath] ON [dbo].[ImageFiles]([StoragePath] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_ImageFiles_Sha256] ON [dbo].[ImageFiles]([Sha256] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_InvMov_ProductId] ON [dbo].[InventoryMovement]([ProductId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_InvMov_SaleId] ON [dbo].[InventoryMovement]([SaleId] ASC) WHERE ([SaleId] IS NOT NULL);
GO

CREATE NONCLUSTERED INDEX [IX_InvMov_Type] ON [dbo].[InventoryMovement]([MovementType] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Product_Name] ON [dbo].[Product]([Name] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Product_UnitId] ON [dbo].[Product]([UnitId] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_Product_Code] ON [dbo].[Product]([Code] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_Product_ImageId_NotNull] ON [dbo].[Product]([ImageId] ASC) WHERE ([ImageId] IS NOT NULL);
GO

CREATE NONCLUSTERED INDEX [IX_RefreshToken_UserId] ON [dbo].[RefreshToken]([UserId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_RefreshToken_ExpiresAtUtc] ON [dbo].[RefreshToken]([ExpiresAtUtc] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Sale_DateUtc] ON [dbo].[Sale]([DateUtc] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_Sale_TicketNumber] ON [dbo].[Sale]([TicketNumber] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_SaleItem_ProductId] ON [dbo].[SaleItem]([ProductId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_SaleItem_SaleId] ON [dbo].[SaleItem]([SaleId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_User_Email] ON [dbo].[User]([Email] ASC);
GO

ALTER TABLE [dbo].[Unit] ADD CONSTRAINT [DF_Unit_Id] DEFAULT (newid()) FOR [Id];
GO

ALTER TABLE [dbo].[Unit] ADD CONSTRAINT [DF_Unit_CreatedAtUtc] DEFAULT (sysutcdatetime()) FOR [CreatedAtUtc];
GO

ALTER TABLE [dbo].[ImageFiles] ADD CONSTRAINT [DF_ImageFiles_CreatedAtUtc] DEFAULT (sysutcdatetime()) FOR [CreatedAtUtc];
GO

ALTER TABLE [dbo].[User] ADD CONSTRAINT [DF_User_Id] DEFAULT (newid()) FOR [Id];
GO

ALTER TABLE [dbo].[User] ADD CONSTRAINT [DF_User_IsAdmin] DEFAULT ((0)) FOR [IsAdmin];
GO

ALTER TABLE [dbo].[User] ADD CONSTRAINT [DF_User_Active] DEFAULT ((1)) FOR [Active];
GO

ALTER TABLE [dbo].[User] ADD CONSTRAINT [DF_User_CreatedAtUtc] DEFAULT (sysutcdatetime()) FOR [CreatedAtUtc];
GO

ALTER TABLE [dbo].[Sale] ADD CONSTRAINT [DF_Sale_Id] DEFAULT (newid()) FOR [Id];
GO

ALTER TABLE [dbo].[Sale] ADD CONSTRAINT [DF_Sale_DateUtc] DEFAULT (sysutcdatetime()) FOR [DateUtc];
GO

ALTER TABLE [dbo].[Sale] ADD CONSTRAINT [DF_Sale_Total] DEFAULT ((0)) FOR [Total];
GO

ALTER TABLE [dbo].[Sale] ADD CONSTRAINT [DF_Sale_CreatedAtUtc] DEFAULT (sysutcdatetime()) FOR [CreatedAtUtc];
GO

ALTER TABLE [dbo].[Product] ADD CONSTRAINT [DF_Product_Id] DEFAULT (newid()) FOR [Id];
GO

ALTER TABLE [dbo].[Product] ADD CONSTRAINT [DF_Product_PriceSale] DEFAULT ((0)) FOR [PriceSale];
GO

ALTER TABLE [dbo].[Product] ADD CONSTRAINT [DF_Product_PriceBuy] DEFAULT ((0)) FOR [PriceBuy];
GO

ALTER TABLE [dbo].[Product] ADD CONSTRAINT [DF_Product_CreatedAtUtc] DEFAULT (sysutcdatetime()) FOR [CreatedAtUtc];
GO

ALTER TABLE [dbo].[RefreshToken] ADD CONSTRAINT [DF_RefreshToken_Id] DEFAULT (newid()) FOR [Id];
GO

ALTER TABLE [dbo].[RefreshToken] ADD CONSTRAINT [DF_RefreshToken_CreatedAtUtc] DEFAULT (sysutcdatetime()) FOR [CreatedAtUtc];
GO

ALTER TABLE [dbo].[RefreshToken] ADD CONSTRAINT [DF_RefreshToken_IsRevoked] DEFAULT ((0)) FOR [IsRevoked];
GO

ALTER TABLE [dbo].[SaleItem] ADD CONSTRAINT [DF_SaleItem_Id] DEFAULT (newid()) FOR [Id];
GO

ALTER TABLE [dbo].[SaleItem] ADD CONSTRAINT [DF_SaleItem_CreatedAtUtc] DEFAULT (sysutcdatetime()) FOR [CreatedAtUtc];
GO

ALTER TABLE [dbo].[InventoryMovement] ADD CONSTRAINT [DF_InvMov_Id] DEFAULT (newid()) FOR [Id];
GO

ALTER TABLE [dbo].[InventoryMovement] ADD CONSTRAINT [DF_InvMov_CreatedAtUtc] DEFAULT (sysutcdatetime()) FOR [CreatedAtUtc];
GO

ALTER TABLE [dbo].[Product] ADD CONSTRAINT [FK_Product_Unit] FOREIGN KEY([UnitId]) REFERENCES [dbo].[Unit] ([Id]);
GO

ALTER TABLE [dbo].[Product] ADD CONSTRAINT [FK_Product_ImageFiles_ImageId] FOREIGN KEY([ImageId]) REFERENCES [dbo].[ImageFiles] ([Id]) ON DELETE SET NULL;
GO

ALTER TABLE [dbo].[RefreshToken] ADD CONSTRAINT [FK_RefreshToken_User] FOREIGN KEY([UserId]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [dbo].[SaleItem] ADD CONSTRAINT [FK_SaleItem_Sale] FOREIGN KEY([SaleId]) REFERENCES [dbo].[Sale] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [dbo].[SaleItem] ADD CONSTRAINT [FK_SaleItem_Product] FOREIGN KEY([ProductId]) REFERENCES [dbo].[Product] ([Id]);
GO

ALTER TABLE [dbo].[InventoryMovement] ADD CONSTRAINT [FK_InventoryMovement_Product] FOREIGN KEY([ProductId]) REFERENCES [dbo].[Product] ([Id]);
GO

ALTER TABLE [dbo].[InventoryMovement] ADD CONSTRAINT [FK_InventoryMovement_Sale] FOREIGN KEY([SaleId]) REFERENCES [dbo].[Sale] ([Id]);
GO

ALTER TABLE [dbo].[Product] ADD CONSTRAINT [CK_Product_Prices_NonNegative] CHECK (([PriceSale] >= (0) AND [PriceBuy] >= (0)));
GO

ALTER TABLE [dbo].[SaleItem] ADD CONSTRAINT [CK_SaleItem_Qty_Positive] CHECK (([Quantity] > (0)));
GO

ALTER TABLE [dbo].[SaleItem] ADD CONSTRAINT [CK_SaleItem_Price_NonNegative] CHECK (([UnitPrice] >= (0)));
GO

ALTER TABLE [dbo].[InventoryMovement] ADD CONSTRAINT [CK_InvMov_Type] CHECK (([MovementType] = (3) OR [MovementType] = (2) OR [MovementType] = (1)));
GO

ALTER TABLE [dbo].[InventoryMovement] ADD CONSTRAINT [CK_InvMov_Qty_Positive] CHECK ((([MovementType] = (2) OR [MovementType] = (1)) AND [Quantity] IS NOT NULL AND [Quantity] > (0) AND [QuantityAdj] IS NULL OR [MovementType] = (3) AND [QuantityAdj] IS NOT NULL AND [Quantity] IS NULL));
GO

CREATE VIEW [dbo].[vw_ProductStockOnHand]
AS
SELECT
    p.Id AS ProductId,
    p.Code,
    p.Name,
    SUM(
        CASE
            WHEN im.MovementType = 1 THEN ISNULL(im.Quantity, 0)
            WHEN im.MovementType = 2 THEN ISNULL(-im.Quantity, 0)
            WHEN im.MovementType = 3 THEN ISNULL(im.QuantityAdj, 0)
            ELSE 0
        END
    ) AS StockOnHand
FROM dbo.Product p
LEFT JOIN dbo.InventoryMovement im ON im.ProductId = p.Id
GROUP BY p.Id, p.Code, p.Name;
GO