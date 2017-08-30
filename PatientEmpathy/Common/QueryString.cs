using System;
using System.Collections.Generic;

namespace PatientEmpathy.Common
{
    public class QueryString
    {
        public static string IsPatientDischarge(string epiNo)
        {
            var result = @"
            
                Select PAADM_DischgDate
                From Pa_Adm
                Where Paadm_AdmNo = '{epiNo}'

            ";

            result = result.Replace("{epiNo}", epiNo);


            return result;
        }

        public static string GetEpisodeInquiry(string hn)
        {
            var result = @"

                SELECT PAADM_ADMNo
                ,PAADM_AdmDate
                ,PAADM_AdmTime
                ,PAADM_DepCode_DR->CTLOC_Code
                ,PAADM_DepCode_DR->CTLOC_Desc
                ,PAADM_AdmDocCodeDR->CTPCP_Code
                ,PAADM_AdmDocCodeDR->CTPCP_Desc
                ,PAADM_Type
                ,PAADM_VisitStatus
                ,PAADM_CurrentWard_DR->WARD_Code
                ,PAADM_CurrentWard_DR->WARD_Desc
                ,PAADM_CurrentRoom_DR->ROOM_Code
                FROM pa_adm
                WHERE PAADM_PAPMI_DR->PAPMI_No = '{hn}'
                AND PAADM_AdmDate = CURRENT_DATE

            ";

            result = result.Replace("{hn}", hn);

            return result;
        }

        public static string GetPatientInfo(string hn)
        {
            var result = @"

                SELECT PAPMI_No
                ,PAPMI_Title_DR->TTL_Desc
                ,PAPMI_Name,PAPMI_Name2
                ,PAPMI_DOB
                ,PAPMI_PAPER_DR->PAPER_AgeYr
                ,PAPMI_PAPER_DR->PAPER_AgeMth	
                ,PAPMI_PAPER_DR->PAPER_AgeDay
                ,PAPMI_Sex_DR->CTSEX_Desc
                ,PAPMI_PAPER_DR->PAPER_StName
                ,PAPMI_PAPER_DR->PAPER_CityArea_DR->CITAREA_Desc
                ,PAPMI_PAPER_DR->PAPER_CityCode_DR->CTCIT_Desc
                ,PAPMI_PAPER_DR->PAPER_CT_Province_DR->PROV_Desc
                ,PAPMI_PAPER_DR->PAPER_Zip_DR->CTZIP_Code
                ,PAPMI_PAPER_DR->PAPER_TelH 
                FROM pa_patmas
                WHERE PAPMI_No = '{hn}'

            ";

            result = result.Replace("{hn}", hn);

            return result;
        }

        public static string GetPatientHn(string epiNo)
        {
            var result = @"SELECT PAADM_PAPMI_DR->Papmi_No 
                            FROM pa_adm WHERE PAADM_ADMNo = '{epiNo}'";
            result = result.Replace("{epiNo}", epiNo);

            return result;
        }

        public static string GetIcd9(string epiNo)
        {
            var result = @"

                SELECT RBOP_PAADM_DR->OR_Anaesthesia->OR_Anaest_Operation->ANAOP_Type_DR->OPER_Code
                ,RBOP_PAADM_DR->OR_Anaesthesia->OR_Anaest_Operation->ANAOP_Type_DR->OPER_Desc 
                FROM RB_OperatingRoom
                WHERE RBOP_PAADM_DR->PAADM_ADMNo = '{epiNo}'

            ";


            result = result.Replace("{epiNo}", epiNo);

            return result;
        }

        public static string GetIcd10(string epiNo)
        {
            var result = @"

                SELECT PAADM_MainMRADM_DR->MR_Diagnos->MRDIA_ICDCode_DR->MRCID_Code
                ,PAADM_MainMRADM_DR->MR_Diagnos->MRDIA_ICDCode_DR->MRCID_Desc
                FROM PA_Adm
                where paadm_admno = '{epiNo}'

            ";


            result = result.Replace("{epiNo}", epiNo);

            return result;
        }

        public static string GetAppointments(string hn)
        {
            const string result = @"

            SELECT CONVERT(VARCHAR(10),APPT_AS_ParRef->AS_Date,103) AS_Date
            ,APPT_AS_ParRef->AS_Date AS_Date1
            ,APPT_AS_ParRef->AS_SessStartTime
            ,APPT_Status
            ,APPT_Adm_DR->PAADM_VisitStatus
            ,APPT_AS_ParRef->AS_RES_ParRef->RES_CTLOC_DR->CTLOC_Code
            ,APPT_AS_ParRef->AS_RES_ParRef->RES_CTLOC_DR->CTLOC_Desc
            ,APPT_AS_ParRef->AS_RES_ParRef->RES_CTPCP_DR->CTPCP_Desc 
            ,APPT_RBCServ_DR->SER_Desc
            FROM RB_Appointment
            WHERE APPT_Adm_DR->PAADM_PAPMI_DR->PAPMI_No = ?
            and APPT_AS_ParRef->AS_Date >= getdate() and APPT_Adm_DR->PAADM_VisitStatus = 'P'
            and APPT_AS_ParRef->AS_Date IN
            (
            	SELECT DISTINCT TOP 2 APPT_AS_ParRef->AS_Date
	            FROM
	            RB_Appointment
	            WHERE
	            APPT_Adm_DR -> PAADM_PAPMI_DR -> PAPMI_No = ?
	            AND APPT_Adm_DR -> PAADM_ADMNO IS  NULL
	            AND APPT_Adm_DR -> PAADM_VisitStatus <> 'A'
	            and APPT_Adm_DR -> PAADM_ADMDATE >= GetDate()
	            Order by
	            APPT_Adm_DR -> PAADM_ADMDATE ,
	            APPT_Adm_DR -> PAADM_ADMTIME 
            )
            order by APPT_AS_ParRef->AS_Date,APPT_AS_ParRef->AS_SessStartTime

            ";

            return result;
        }

        public static string GetAppointmentsCurrent(string hn)
        {
            const string result = @"
            
           SELECT CONVERT(VARCHAR(10),APPT_AS_ParRef->AS_Date,103) AS_Date
            ,APPT_AS_ParRef->AS_Date AS_Date1
            ,APPT_AS_ParRef->AS_SessStartTime
            ,APPT_Status
            ,APPT_Adm_DR->PAADM_VisitStatus
            ,APPT_AS_ParRef->AS_RES_ParRef->RES_CTLOC_DR->CTLOC_Code
            ,APPT_AS_ParRef->AS_RES_ParRef->RES_CTLOC_DR->CTLOC_Desc
            ,APPT_AS_ParRef->AS_RES_ParRef->RES_CTPCP_DR->CTPCP_Desc 
            ,APPT_RBCServ_DR->SER_Desc
            FROM RB_Appointment
            WHERE APPT_Adm_DR->PAADM_PAPMI_DR->PAPMI_No = ?
            and APPT_Adm_DR->PAADM_VisitStatus = 'A'
            order by APPT_AS_ParRef->AS_Date , APPT_AS_ParRef->AS_SessStartTime 

            ";
            return result;
        }

        public static string GetAppointmentsPast(string hn)
        {
            const string result = @"
            SELECT
            CONVERT(
            VARCHAR(10),
            APPT_AS_ParRef -> AS_Date,
            103
            ) AS_Date,
            APPT_AS_ParRef -> AS_Date AS_Date1,
            APPT_AS_ParRef -> AS_SessStartTime,
            APPT_Status,
            APPT_Adm_DR -> PAADM_VisitStatus,
            APPT_AS_ParRef -> AS_RES_ParRef -> RES_CTLOC_DR -> CTLOC_Code,
            APPT_AS_ParRef -> AS_RES_ParRef -> RES_CTLOC_DR -> CTLOC_Desc,
            APPT_AS_ParRef -> AS_RES_ParRef -> RES_CTPCP_DR -> CTPCP_Desc,
            APPT_RBCServ_DR -> SER_Desc
            FROM
            RB_Appointment
            WHERE
            APPT_Adm_DR -> PAADM_PAPMI_DR -> PAPMI_No = ?
            and APPT_Adm_DR -> PAADM_ADMNO in(
            SELECT
            DISTINCT TOP 2 APPT_Adm_DR -> PAADM_ADMNO
            FROM
            RB_Appointment
            WHERE
            APPT_Adm_DR -> PAADM_PAPMI_DR -> PAPMI_No = ?
            AND APPT_Adm_DR -> PAADM_ADMNO IS NOT NULL
            AND APPT_Adm_DR -> PAADM_VisitStatus <> 'A'
            Order by
            APPT_Adm_DR -> PAADM_ADMDATE DESC,
            APPT_Adm_DR -> PAADM_ADMTIME DESC
            )
            Order by
            APPT_AS_ParRef -> AS_Date DESC,
            APPT_AS_ParRef -> AS_SessStartTime DESC      
                                                                   
            ";
            return result;
        }

        public static string GetAllergy(string hn)
        {
            var alg = @"

                SELECT ALG_Comments
                ,ALG_PHCGE_DR->PHCGE_Name
                ,ALG_Type_DR->ALG_Desc
                ,ALG_AllergyGrp_DR->ALGR_Desc
                ,ALG_PHCDRGForm_DR->PHCDF_PHCD_ParRef->PHCD_Name
                ,ALG_Ingred_DR->INGR_Desc
                FROM PA_Allergy
                WHERE ALG_PAPMI_ParRef->PAPMI_No = '{hn}'
                and ALG_Status = 'A'
            ";

            alg = alg.Replace("{hn}", hn);

            return alg;
        }

        public static string GetAllergyCategory(string algName)
        {
            var algCat = @"

                SELECT ALG_Type_DR->MRCAT_Desc
                FROM PAC_Allergy
                Where ALG_Desc like '{algName}%'

            ";

            algCat = algCat.Replace("{algName}", algName);

            return algCat;
        }

        public static string GetLastEpisode(string hn)
        {
            var result = @"

                SELECT top 1 PAADM_ADMNo
                FROM pa_adm
                WHERE PAADM_PAPMI_DR->PAPMI_No = '{hn}' and  PAADM_ADMNo <> ''
                order by PAADM_ADMDATE DESC,PAADM_AdmTime desc,PAADM_Rowid desc 

            ";
            //order by PAADM_ADMDATE DESC,PAADM_Rowid
            result = result.Replace("{hn}", hn);

            return result;
        }

        public static string GetPatient(string date)
        {
            var result = @"

                select ""JsonData"",""Labjson"" from  ""Admission""  where ""DateAdd"" ='{date}' 

            ";

            result = result.Replace("{date}", date);

            return result;
        }

        public static string GetPatient(string date, string locCode)
        {
            var result = @"

                select ""JsonData"",""Labjson"" 
                from  ""Admission""  
                where ""DateAdd"" ='{date}' and ""loc_code"" = '{locCode}'

            ";

            result = result.Replace("{date}", date);
            result = result.Replace("{locCode}", locCode);

            return result;
        }

        public static string GetPatientByHn(string hn)
        {
            var result = @"

                select ""JsonData"",""Labjson"" from  ""Admission""  where ""HN"" ='{hn}' 

            ";

            result = result.Replace("{hn}", hn);

            return result;
        }

        public static Tuple<string, Dictionary<string, string>> GetLocation(string site, string type="")
        {
            var dic = new Dictionary<string, string>();
            string result;
            site = site.ToUpper().Trim();
            type = string.IsNullOrWhiteSpace(type)? "" : type.ToUpper().Trim();

            site = site == "SNH" ? "12" : "11";

            if (string.IsNullOrEmpty(type) || type == "ALL")
            {
                result = @"
                    SELECT CTLOC_Code,CTLOC_Desc
                    FROM CT_Loc
                    WHERE SUBSTRING(CTLOC_Code,0,3) = ? and  CTLOC_DateActiveTo is null
                    order by CTLOC_Desc
                ";
                dic.Add("CTLOC_Code", site);
            }
            else
            {
                result = @"
                    select CTLOC_Code,CTLOC_Desc,CTLOC_RowID ,CTLOC_Hospital_DR,* from ct_LOC
                    where SUBSTRING(CTLOC_Code,0,3) = ? and CTLOC_Type = ? and CTLOC_DateActiveTo is null
                ";
                // C = Cashier, D = Pharmacy
                dic.Add("CTLOC_Code", site);
                dic.Add("CTLOC_Type", type);
            }
            
            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }

        public static string GetRoom(string buId, string ward)
        {
            var result = @"

                SELECT ROOM_ParRef->WARD_Code,ROOM_Room_DR->ROOM_Code,ROOM_Room_DR->ROOM_Desc
                FROM PAC_WardRoom
                WHERE ROOM_ParRef->WARD_Active = 'Y'
                and left(ROOM_ParRef->WARD_Desc,3)<> 'ZZZ'
                and isnull(ROOM_Room_DR->ROOM_Code,'') not in ('','11FN','12FN') 
                and ROOM_ParRef->WARD_Code not in ('11W7R','12LTC') ";
            if (ward != "%" && !string.IsNullOrEmpty(ward))
            {
                result += "and ROOM_ParRef->WARD_Code = '" + ward + "' ";
            }
            else
            {
                result += "and left(ROOM_ParRef->WARD_Code,2) = '" + buId + "' ";
            }
            result += "order by left(ROOM_ParRef->WARD_Code,2),ROOM_Room_DR->ROOM_Code ";


            return result;
        }

        public static string GetWard(string buId)
        {
            var result = @"

                SELECT distinct ROOM_ParRef->WARD_Code,ROOM_ParRef->WARD_Desc
                FROM PAC_WardRoom
                WHERE ROOM_ParRef->WARD_Active = 'Y'
                and left(ROOM_ParRef->WARD_Desc,3)<> 'ZZZ'
                and isnull(ROOM_Room_DR->ROOM_Code,'') not in ('','11FN','12FN') 
                and ROOM_ParRef->WARD_Code not in ('11W7R','12LTC') 
                and left(ROOM_ParRef->WARD_Code,2) = '{buId}' 
                order by left(ROOM_ParRef->WARD_Code,2),ROOM_ParRef->WARD_Code ";

            result = result.Replace("{buId}", buId);

            return result;
        }
        public static string GetPatientImage(string hn)
        {
            var result = @"

            select PAPMI_RowId->PAPER_PHOTODocument->docData As ImageHN
            from PA_Patmas where PAPMI_NO ='{hn}'

            ";

            result = result.Replace("{hn}", hn);

            return result;
        }

        public static Tuple<string, Dictionary<string, string>> GetAlertMsg(string hn)
        {
            const string result = @"
                SELECT ALM_AlertCategory_DR->ALERTCAT_Code
                ,ALM_AlertCategory_DR->ALERTCAT_Desc
                ,ALM_Message,ALM_Status
                FROM PA_AlertMsg
                WHERE ALM_PAPMI_ParRef->PAPMI_No = ?
                AND ALM_Status = 'A'
            ";

            //result = result.Replace("{hn}", hn);

            var dic = new Dictionary<string, string> {{"PAPMI_No", hn}};

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }

        public static Tuple<string, Dictionary<string, string>> GetPatientCategory(string hn)
        {
            const string result = @"
                SELECT PAPMI_PatCategory_DR->PCAT_Code, PAPMI_PatCategory_DR->PCAT_Desc 
                FROM PA_PATMAS 
                WHERE PAPMI_No = ?
            ";

            var dic = new Dictionary<string, string> {{"PAPMI_No", hn}};

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }

        public static Tuple<string, Dictionary<string, string>> GetLocationByLineBeacon(string beaconId)
        {
            const string result = @"
                SELECT ""LineBeacon_CODE"", ""Location_LineBeacon"".""CTLOC_CODE"", ""Location"".""CTLOC_DESC"", ""Location"".""CTLOC_FLOOR""
                FROM ""Location_LineBeacon""
                INNER JOIN ""Location"" ON(""Location_LineBeacon"".""CTLOC_CODE"" = ""Location"".""CTLOC_CODE"")
                WHERE ""LineBeacon_CODE"" = @beaconId
            ";

            var dic = new Dictionary<string, string> {{"@beaconId", beaconId}};

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }

        public static Tuple<string, Dictionary<string, string>> GetMedDisch(string hn)
        {
            const string result = @"

                SELECT PAADM_MedDischDate,PAADM_MedDischTime,PAADM_ADMNO
                FROM PA_ADM
                WHERE PAADM_PAPMI_DR-> PAPMI_NO = ?
                AND PAADM_AdmDate = CURRENT_DATE
                AND SUBSTRING(PAADM_ADMNO,0,2) = 'O'
                AND PAADM_VisitStatus in ('A','D')  
            ";

            var dic = new Dictionary<string, string> {{"PAPMI_NO", hn}};

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }
        public static Tuple<string, Dictionary<string, string>> GetFinDisc(string hn)
        {
            const string result = @"

                SELECT PAADM_FinDischDate,PAADM_FinDischTime,PAADM_ADMNO
                FROM PA_ADM
                WHERE PAADM_PAPMI_DR-> PAPMI_NO = ?
                AND PAADM_AdmDate = CURRENT_DATE
                AND SUBSTRING(PAADM_ADMNO,0,2) = 'O'
                AND PAADM_VisitStatus in ('A','D')  
            ";

            var dic = new Dictionary<string, string> {{"PAPMI_NO", hn}};

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }
        public static Tuple<string, Dictionary<string, string>> GetDischg(string hn)
        {
            const string result = @"

                SELECT PAADM_DischgDate,PAADM_DischgTime,PAADM_ADMNO
                FROM PA_ADM
                WHERE PAADM_PAPMI_DR-> PAPMI_NO = ?
                AND PAADM_AdmDate = CURRENT_DATE
                AND SUBSTRING(PAADM_ADMNO,0,2) = 'O'
                AND PAADM_VisitStatus in ('A','D')  
            ";

            var dic = new Dictionary<string, string> {{"PAPMI_NO", hn}};

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }

        public static Tuple<string, Dictionary<string, string>> GetPatientRegisLoc(string hn)
        {
            const string result = @"
            
                SELECT PAADM_DepCode_DR->CTLOC_Code
                FROM PA_ADM
                WHERE PAADM_PAPMI_DR->PAPMI_No = ?
                AND PAADM_AdmDate = CURRENT_DATE
                GROUP BY PAADM_DepCode_DR->CTLOC_Code

            ";

            var dic = new Dictionary<string, string> {{"PAPMI_No", hn}};

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }

        public static Tuple<string, Dictionary<string, string>> HnMedDischList(string listHn)
        {
            var result = @"
            
                SELECT PAADM_PAPMI_DR-> PAPMI_NO
                FROM PA_ADM
                WHERE PAADM_PAPMI_DR-> PAPMI_NO in
                (
                    {listHn}
                )
                AND PAADM_AdmDate = CURRENT_DATE
                AND SUBSTRING(PAADM_ADMNO,0,2) = 'O'
                AND PAADM_VisitStatus in ('A')
                AND PAADM_MedDischDate is not null

            ";

            result = result.Replace("{listHn}", listHn);

            var dic = new Dictionary<string, string>();
            //dic.Add("PAPMI_No", listHn);

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }

        public static Tuple<string, Dictionary<string, string>> DataAllDischsList(string listHn)
        {
            var result = @"
            
                SELECT PAADM_PAPMI_DR->PAPMI_NO
                ,PAADM_ADMNO,PAADM_MedDischDate
                ,PAADM_MedDischTime,PAADM_FinDischDate
                ,PAADM_FinDischTime,PAADM_DischgDate
                ,PAADM_DischgTime
                FROM PA_ADM
                WHERE PAADM_PAPMI_DR-> PAPMI_NO  in
                (
                    {listHn}
                )
                AND PAADM_AdmDate = CURRENT_DATE
                AND SUBSTRING(PAADM_ADMNO,0,2) = 'O'
                AND PAADM_VisitStatus in ('A','D')  

            ";
            result = result.Replace("{listHn}", listHn);
            var dic = new Dictionary<string, string>();

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }
        
        public static Tuple<string, Dictionary<string, string>> PatientSeeToDoctor(string listHn)
        {
            var result = @"

                SELECT APPT_PAPMI_DR->PAPMI_No
				,APPT_Adm_DR->PAADM_DepCode_DR->CTLOC_Code
				FROM RB_Appointment
				where APPT_PAPMI_DR->PAPMI_No in 
                (
				    {listHn}
				)
				and APPT_Adm_DR->PAADM_AdmDate = CURRENT_DATE
				and APPT_Status <> 'X'
				AND APPT_SeenTime is NULL

            ";

            result = result.Replace("{listHn}", listHn);
            var dic = new Dictionary<string, string>();

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }

        public static Tuple<string, Dictionary<string, string>> GetAllPatientBilled(string listHn)
        {
            //var result = @"

            //    SELECT ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_No
            //    ,ARPBL_PAADM_DR->PAADM_ADMNO
            //    ,ARPBL_BillNo
            //    ,ARPBL_DatePrinted
            //    ,ARPBL_TimePrinted
            //    FROM AR_PatientBill
            //    WHERE ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_No in
            //    (
            //     {listHn}
            //    ) 
            //    AND ARPBL_PAADM_DR->PAADM_ADMDATE = CURRENT_DATE 

            //";
            var result = @"

                SELECT ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_No
                ,ARPBL_PAADM_DR->PAADM_ADMNO
                ,ARPBL_BillNo
                ,ARPBL_DatePrinted
                ,ARPBL_TimePrinted,*
                FROM AR_PatientBill
                WHERE ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_No in
                (
	                {listHn}
                ) 
                AND ARPBL_PAADM_DR->PAADM_ADMDATE = CURRENT_DATE 
                AND (ARPBL_BillNo is not null or (ARPBL_BillNo is null and ((ARPBL_TotalPatient +ARPBL_TotalInsCo) > 0)))

            ";

            result = result.Replace("{listHn}", listHn);
            var dic = new Dictionary<string, string>();

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }

        public static Tuple<string, Dictionary<string, string>> GetPharCollect(string listHn)
        {
            var result = @"
                SELECT  OEORI_OEORD_ParRef->OEORD_Adm_DR->PAADM_PAPMI_DR->PAPMI_No
                ,OEORI_OEORD_ParRef->OEORD_Adm_DR->PAADM_ADMNo
                ,OEORI_PrescNo ,OEORI_PharmacyStatus ,OEORI_ItemStat_DR
                FROM OE_OrdItem
                WHERE OEORI_PRN in 
                (
                    {listHn}
                )
                AND OEORI_OEORD_ParRef->OEORD_Adm_DR->PAADM_ADMDATE = CURRENT_DATE 
            ";

            result = result.Replace("{listHn}", listHn);

            return  new Tuple<string, Dictionary<string, string>>(result, new Dictionary<string, string>());
        }
    }
}