SELECT 
    fk.name AS ForeignKeyName,
    t.name AS TableName,
    c.name AS ColumnName
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fkc
    ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.tables AS t
    ON fkc.parent_object_id = t.object_id
INNER JOIN sys.columns AS c
    ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
WHERE t.name = 'Categories';
