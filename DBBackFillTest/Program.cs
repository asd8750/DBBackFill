using System;
using System.Collections.Generic;
using System.Linq;

using DBBackfill;

namespace FSBackfillTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //string srcServer = @"EDR1SQL01N901";
            //string srcDbName = "Performance_Global07";
            //string srcTableSchema = "Performance";
            //string srcTableName = "60PerformanceCurrentTransducers";

            //string dstServer = @"EDR1SQL01N901";
            //string dstDbName = "Performance_Global07";
            //string dstTableSchema = "DBA-Stg";
            //string dstTableName = "60PerformanceCurrentTransducers";

            //string srcServer = @"EDR1SQL01V700\ODSPROD";
            //string srcDbName = "ODS";
            //string srcTableSchema = "dbo";
            //string srcTableName = "PanelCcm2Detail";

            string srcServer = @"EDR1SQL01V700\ODSPROD";
            string srcDbName = "ODS";
            string srcTableSchema = "dbo";
            string srcTableName = "vwPanelEfficiency";

            string dstServer = srcServer;
            string dstDbName = srcDbName;
            string dstTableSchema = srcTableSchema;
            string dstTableName = "_createPanelEfficiency"; //  srcTableName;

            //string dstServer = @"PBG1SQL01S536\DBA_130_S536";
            //string dstDbName = "FSSqlServerStatus";
            //string dstTableSchema = "DBA-Stg";
            //string dstTableName = "PanelCcm2Detail";

            //string pubToSync = "Backfill - Controller";

            string TOD = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            BackfillCtl bfcHub = new BackfillCtl("DBA", 1);
            bfcHub.DebugFile = $"Backfill_{srcTableSchema}_{srcTableName}_{TOD}.log";

            string Version = bfcHub.Version;

            bfcHub.OpenInstance(srcServer);
            bfcHub.OpenInstance(dstServer);
            bfcHub.CommandTimeout = 1200; // SQL command timeout value

            DatabaseInfo srcDB = bfcHub[srcServer][srcDbName];
            DatabaseInfo dstDB = bfcHub[dstServer][dstDbName];

            TableInfo srcTbl1 = srcDB.GetTable(srcTableSchema, srcTableName);
            //TableInfo joinSrcTbl = srcDB.GetTable(srcRLTableSchema, srcRLTableName);

            TableInfo dstTbl1 = dstDB.GetTable(dstTableSchema, dstTableName);

            //srcTbl1["ReadTime"].LoadExpression = "ISNULL(ReadTime, LastModifiedTime)";

            //FetchKeyBoundary fkb1 = FetchKeyHelpers.CreateFetchKeyComplete(srcTbl1, "FBDID");
            FetchKeyBoundary fkb1 = FetchKeyHelpers.CreateFetchKeyComplete(srcTbl1, "SubId");

            //ExtraSrcColumn ptDate = new ExtraSrcColumn(joinSrcTbl, "ReadTimeLocal", "LineageId, ");

            //fkb1.SetRelatedTable(joinSrcTbl, "Ccm2HeaderId", "Ccm2HeaderId", "OuterJoin");
            //fkb1.AddFetchCol(joinSrcTbl["ReadTimeLocal"], "ISNULL(SRC2.ReadTimeLocal,GETDATE())");

            //fkb1.ModifyFetchCol("StringValue", "CASE WHEN LEN([__X__])< 100000 THEN [__X__] ELSE '' END");

            // fkb1.IgnoreFetchCol("index_handle");
            fkb1.TableHint = "NOLOCK";
            fkb1.AndWhere = "";


            fkb1.FlgSelectByPartition = true;
            //fkb1.RestartPartition = 6;
            //fkb1.RestartKeys = new List<object>() { 4512653349 };
            //fkb1.AddRestartKey((Int64)3789734428622848);
            //fkb1.AddRestartKey(DateTime.Parse("2017-10-18 06:34:05.023"));
            //fkb1.AddRestartKey((Int16) 6);
            //fkb1.AddRestartKey((Int32) 1412968160);
            //fkb1.AddRestartKey((Int32) 1279270);

            //fkb1.AddEndKey(6354221593);

            //    $fkb1.FillType = 1
            fkb1.FillTypeName = "BulkInsert";

            //$restartTime = Get - Date - Date "2015/04/30 12:10:00.000"
            //fkb1.AddRestartKey(DateTime.Parse("2015/04/30 12:10:00.000"));
            //fkb1.RestartPartition = 173;

            bfcHub.BackfillData(srcTbl1, dstTbl1, null, fkb1, 102400); //, fkb1.FKeyColNames, fkb1.FKeyColNames);
            //bfcHub.BackfillData(srcTbl1, dstTbl1, null, fkb1, 102400, fkb1.FKeyColNames, fkb1.FKeyColNames);

            //foreach (Publication pub in srcDB.Publications)
            //{
            //Publication pub = srcDB.Publications[pubToSync];
            //Console.WriteLine("Pub: {0}", pub.Name);
            //}

        }
    }
}
