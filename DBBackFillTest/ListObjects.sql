
WITH TVL AS (
		SELECT	OBJ.[object_id]  AS ObjectID,
				SCH.[name] AS SchemaName,
				OBJ.[name] AS TableName,
				OBJ.[type] AS ObjType,
				COL.[name] AS ColName,
				COL.column_id AS ColumnID,
				TYP.[name] AS DataType,
				COL.max_length ,
				COL.[precision] ,
				COL.[scale] ,
				COL.is_xml_document ,
				COL.is_nullable ,
				COL.is_identity ,
				COL.is_computed ,
				IDX.[name] AS IndexName,
				IDX.index_id AS IndexID,
				IDX.is_unique,
				IDXC.key_ordinal AS IndexKeyOrdinal,
				IDXC.column_id AS IndexColumnID,
				DENSE_RANK() OVER ( PARTITION BY OBJ.[object_id] ORDER BY  ISNULL(IDX.is_unique, 0) DESC, ISNULL(IDX.index_id, 99) ) AS IndexPriority

			FROM 	sys.objects	OBJ
				INNER JOIN  sys.schemas SCH
					ON (OBJ.[schema_id] = SCH.[schema_id])
				INNER JOIN	sys.columns COL 
					ON (OBJ.[object_id] = COL.[object_id])
				INNER JOIN sys.types TYP 
					ON ( COL.user_type_id = TYP.user_type_id )
				LEFT OUTER JOIN (
					sys.indexes IDX
						INNER JOIN sys.index_columns IDXC
							ON (IDX.[object_id] = IDXC.[object_id])
								AND (IDX.index_id = IDXC.index_id)		
					)
					ON (OBJ.[object_id] = IDX.[object_id])
						AND (IDXC.column_id = COL.column_id)
			 
			WHERE	(OBJ.[type] IN ('U', 'V')) 
				AND (OBJ.is_ms_shipped = 0)
				
			ORDER BY SchemaName, TableName,  IndexPriority,  IDX.index_id, ColumnID, IDXC.key_ordinal
			)
	
	
	
	
	
	
		SELECT   *,
						ROW_NUMBER() OVER (PARTITION BY IDXC2.[object_id], IDXC2.column_id ORDER BY IDXC2.IndexPriority) AS ColPrio
               FROM     ( SELECT    DISTINCT
									TBL.[object_id] ,
									TBL.[type] AS ObjType,
									IDX.[name] AS IndexName,
                                    IDX.index_id ,
									IDX.[type] AS index_type,
                                    IDX.is_unique ,
                                    COL.column_id ,
                                    COALESCE(IDC.key_ordinal, 0) AS key_ordinal ,
                                    COALESCE(IDC.is_descending_key , 0) AS is_descending_key,
                                    COALESCE(IDC.partition_ordinal, 0) AS partition_ordinal
                                    ,DENSE_RANK() OVER ( PARTITION BY TBL.[object_id] ORDER BY IDX.is_unique DESC, IDX.index_id ) AS IndexPriority
	                         FROM (sys.objects TBL 
		                        INNER JOIN sys.columns COL ON (TBL.[object_id] = COL.[object_id]))
		                        LEFT OUTER JOIN 
	                                (sys.indexes IDX
		                                INNER JOIN sys.index_columns IDC 
                                            ON ( IDX.[object_id] = IDC.[object_id] ) AND ( IDX.[index_id] = IDC.[index_id] )
                                    )
						        ON (TBL.[object_id] = IDX.[object_id] ) AND (COL.column_id = IDC.column_id)
                            WHERE (TBL.[type] IN ('U', 'V'))
                        ) IDXC2				
