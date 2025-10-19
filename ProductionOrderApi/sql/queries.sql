SELECT
    ProductCode,
    SUM(QuantityProduced) AS TotalProduzido
FROM
    ProductionOrder
GROUP BY
    ProductCode;

SELECT
    (
        COUNT(
            CASE
                WHEN Status = 'Finalizada' THEN 1
            END
        ) * 100.0 / COUNT(*)
    ) AS PercentualFinalizadas
FROM
    ProductionOrder;

UPDATE ProductionOrder
SET
    EndDate = GETDATE ()
WHERE
    Status = 'Finalizada'
    AND EndDate IS NULL;

SELECT
    OrderNumber,
    ProductCode,
    QuantityPlanned,
    QuantityProduced,
    (QuantityProduced * 100.0 / QuantityPlanned) AS PercentualConcluido
FROM
    ProductionOrder
WHERE
    Status IN ('EmProducao', 'Finalizada');