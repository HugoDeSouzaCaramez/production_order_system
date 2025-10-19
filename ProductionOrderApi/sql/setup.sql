IF NOT EXISTS (
    SELECT
        *
    FROM
        sys.databases
    WHERE
        name = 'ProductionOrderDb'
) BEGIN CREATE DATABASE ProductionOrderDb;

PRINT 'Banco ProductionOrderDb criado com sucesso.';

END ELSE BEGIN PRINT 'Banco ProductionOrderDb já existe.';

END GO USE ProductionOrderDb;

GO IF NOT EXISTS (
    SELECT
        *
    FROM
        sysobjects
    WHERE
        name = 'Product'
        AND xtype = 'U'
)
CREATE TABLE
    Product (
        Id INT IDENTITY (1, 1) PRIMARY KEY,
        Code NVARCHAR (50) NOT NULL UNIQUE,
        Description NVARCHAR (100) NOT NULL
    );

GO IF NOT EXISTS (
    SELECT
        *
    FROM
        sysobjects
    WHERE
        name = 'Resource'
        AND xtype = 'U'
)
CREATE TABLE
    Resource (
        Id INT IDENTITY (1, 1) PRIMARY KEY,
        Code NVARCHAR (50) NOT NULL UNIQUE,
        Description NVARCHAR (100) NOT NULL,
        Status NVARCHAR (20) CHECK (Status IN ('Disponível', 'EmUso', 'Parado'))
    );

GO IF NOT EXISTS (
    SELECT
        *
    FROM
        sysobjects
    WHERE
        name = 'ProductionOrder'
        AND xtype = 'U'
)
CREATE TABLE
    ProductionOrder (
        Id INT IDENTITY (1, 1) PRIMARY KEY,
        OrderNumber NVARCHAR (50) NOT NULL,
        ProductCode NVARCHAR (50) NOT NULL,
        QuantityPlanned INT NOT NULL,
        QuantityProduced INT DEFAULT 0,
        Status NVARCHAR (20) CHECK (
            Status IN ('Planejada', 'EmProducao', 'Finalizada')
        ),
        StartDate DATETIME NOT NULL DEFAULT GETDATE (),
        EndDate DATETIME NULL
    );

GO IF NOT EXISTS (
    SELECT
        *
    FROM
        sysobjects
    WHERE
        name = 'ProductionLog'
        AND xtype = 'U'
)
CREATE TABLE
    ProductionLog (
        Id INT IDENTITY (1, 1) PRIMARY KEY,
        ProductionOrderId INT NOT NULL,
        ResourceId INT NULL,
        Quantity INT NOT NULL,
        Timestamp DATETIME DEFAULT GETDATE (),
        FOREIGN KEY (ProductionOrderId) REFERENCES ProductionOrder (Id),
        FOREIGN KEY (ResourceId) REFERENCES Resource (Id)
    );

GO IF NOT EXISTS (
    SELECT
        *
    FROM
        sys.check_constraints
    WHERE
        name = 'CHK_QuantityProduced'
) BEGIN
ALTER TABLE ProductionOrder ADD CONSTRAINT CHK_QuantityProduced CHECK (QuantityProduced <= QuantityPlanned);

PRINT 'Constraint CHK_QuantityProduced adicionada.';

END IF NOT EXISTS (
    SELECT
        *
    FROM
        sys.check_constraints
    WHERE
        name = 'CHK_EndDate'
) BEGIN
ALTER TABLE ProductionOrder ADD CONSTRAINT CHK_EndDate CHECK (
    EndDate IS NULL
    OR EndDate > StartDate
);

PRINT 'Constraint CHK_EndDate adicionada.';

END GO IF NOT EXISTS (
    SELECT
        1
    FROM
        Product
    WHERE
        Code = 'PROD-001'
) BEGIN
INSERT INTO
    Product (Code, Description)
VALUES
    ('PROD-001', 'Placa de Circuito SMT'),
    ('PROD-002', 'Carcaça Injetada'),
    ('PROD-003', 'Módulo Eletrônico Montado'),
    ('PROD-004', 'Kit Embalado'),
    ('PROD-005', 'Fonte de Alimentação');

PRINT 'Dados inseridos na tabela Product.';

END ELSE BEGIN PRINT 'Dados já existem na tabela Product.';

END GO IF NOT EXISTS (
    SELECT
        1
    FROM
        Resource
    WHERE
        Code = 'RES-001'
) BEGIN
INSERT INTO
    Resource (Code, Description, Status)
VALUES
    ('RES-001', 'Linha SMT 01', 'Disponível'),
    ('RES-002', 'Linha Injeção 02', 'EmUso'),
    ('RES-003', 'Montagem Final 01', 'Disponível'),
    ('RES-004', 'Embalagem 01', 'Parado'),
    ('RES-005', 'Teste Elétrico 01', 'Disponível');

PRINT 'Dados inseridos na tabela Resource.';

END ELSE BEGIN PRINT 'Dados já existem na tabela Resource.';

END GO IF NOT EXISTS (
    SELECT
        1
    FROM
        ProductionOrder
    WHERE
        OrderNumber = 'ORD-1001'
) BEGIN
INSERT INTO
    ProductionOrder (
        OrderNumber,
        ProductCode,
        QuantityPlanned,
        QuantityProduced,
        Status,
        StartDate,
        EndDate
    )
VALUES
    (
        'ORD-1001',
        'PROD-001',
        1000,
        250,
        'EmProducao',
        DATEADD (HOUR, -5, GETDATE ()),
        NULL
    ),
    (
        'ORD-1002',
        'PROD-002',
        500,
        500,
        'Finalizada',
        DATEADD (DAY, -2, GETDATE ()),
        DATEADD (DAY, -1, GETDATE ())
    ),
    (
        'ORD-1003',
        'PROD-003',
        800,
        0,
        'Planejada',
        GETDATE (),
        NULL
    ),
    (
        'ORD-1004',
        'PROD-004',
        1200,
        100,
        'EmProducao',
        DATEADD (HOUR, -2, GETDATE ()),
        NULL
    ),
    (
        'ORD-1005',
        'PROD-005',
        300,
        300,
        'Finalizada',
        DATEADD (DAY, -1, GETDATE ()),
        GETDATE ()
    );

PRINT 'Dados inseridos na tabela ProductionOrder.';

END ELSE BEGIN PRINT 'Dados já existem na tabela ProductionOrder.';

END GO PRINT 'Setup completo! Banco criado e populado com dados iniciais.';