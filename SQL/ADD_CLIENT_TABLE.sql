USE [Invensa]
GO

/* 
 * PLAN DE IMPLEMENTACION DE LOGICA DE CLIENTES
 * 1. Crear la tabla [Client] con campos profesionales de contacto e identificacion.
 * 2. Modificar la tabla [Sale] para asociar un cliente (opcional).
 * 3. Agregar indices para optimizar busquedas por cliente y por identificador fiscal.
 */

-- 1. CREACION DE LA TABLA CLIENTE
CREATE TABLE [dbo].[Client](
    [Id] [uniqueidentifier] NOT NULL,
    [Identifier] [nvarchar](20) NULL, -- RFC, NIT, DNI, etc.
    [Name] [nvarchar](150) NOT NULL,
    [Email] [nvarchar](100) NULL,
    [Phone] [nvarchar](20) NULL,
    [Address] [nvarchar](250) NULL,
    [Active] [bit] NOT NULL,
    [CreatedAtUtc] [datetime2](3) NOT NULL,
    [UpdatedAtUtc] [datetime2](3) NULL,
    [RowVersion] [timestamp] NOT NULL,
    CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Valores por defecto para la tabla Client
ALTER TABLE [dbo].[Client] ADD CONSTRAINT [DF_Client_Id] DEFAULT (newid()) FOR [Id];
GO
ALTER TABLE [dbo].[Client] ADD CONSTRAINT [DF_Client_Active] DEFAULT ((1)) FOR [Active];
GO
ALTER TABLE [dbo].[Client] ADD CONSTRAINT [DF_Client_CreatedAtUtc] DEFAULT (sysutcdatetime()) FOR [CreatedAtUtc];
GO

-- Indice unico para el Identificador (solo si no es nulo) para evitar duplicados de clientes registrados
CREATE UNIQUE NONCLUSTERED INDEX [UX_Client_Identifier] ON [dbo].[Client]([Identifier] ASC) WHERE ([Identifier] IS NOT NULL);
GO

-- 2. MODIFICACION DE LA TABLA SALE PARA ASOCIAR CLIENTE
ALTER TABLE [dbo].[Sale] 
ADD [ClientId] [uniqueidentifier] NULL; -- Es opcional (NULL) como se solicito
GO

-- 3. AGREGAR LLAVE FORANEA
ALTER TABLE [dbo].[Sale]  WITH CHECK ADD  CONSTRAINT [FK_Sale_Client] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Client] ([Id]);
GO

ALTER TABLE [dbo].[Sale] CHECK CONSTRAINT [FK_Sale_Client];
GO

-- Indice para optimizar busquedas de ventas por cliente
CREATE NONCLUSTERED INDEX [IX_Sale_ClientId] ON [dbo].[Sale]([ClientId] ASC) WHERE ([ClientId] IS NOT NULL);
GO

/* 
 * NOTAS DE LOGICA DE NEGOCIO:
 * - El campo 'Identifier' permite almacenar registros legales/fiscales (RFC, NIT, etc).
 * - La relacion es opcional: Una venta puede o no tener un cliente.
 * - 'Active' permite desactivar clientes sin borrar su historial de ventas (integridad referencial).
 */
