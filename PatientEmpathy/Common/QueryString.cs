using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientEmpathy.Common
{
    public class QueryString
    {
        public static string IsPatientDischarge(string epiNo)
        {
            string result = @"
            
                Select PAADM_DischgDate
                From Pa_Adm
                Where Paadm_AdmNo = '{epiNo}'

            ";

            result = result.Replace("{epiNo}", epiNo);


            return result;
        }

        public static string GetEpisodeInquiry(string hn)
        {
            string result = @"

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
                and convert(varchar(10),PAADM_AdmDate ,120) = convert(varchar(10),Getdate(),120)

            ";

            result = result.Replace("{hn}", hn);

            return result;
        }

        public static string GetPatientInfo(string hn)
        {
            string result = @"

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

        public static string GetPatientHN(string epiNo)
        {
            string result = @"SELECT PAADM_PAPMI_DR->Papmi_No 
                            FROM pa_adm WHERE PAADM_ADMNo = '{epiNo}'";
            result = result.Replace("{epiNo}", epiNo);

            return result;
        }

        public static string GetICD9(string epiNo)
        {
            string result = @"

                SELECT RBOP_PAADM_DR->OR_Anaesthesia->OR_Anaest_Operation->ANAOP_Type_DR->OPER_Code
                ,RBOP_PAADM_DR->OR_Anaesthesia->OR_Anaest_Operation->ANAOP_Type_DR->OPER_Desc 
                FROM RB_OperatingRoom
                WHERE RBOP_PAADM_DR->PAADM_ADMNo = '{epiNo}'

            ";


            result = result.Replace("{epiNo}", epiNo);

            return result;
        }

        public static string GetICD10(string epiNo)
        {
            string result = @"

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
            string result = @"


            SELECT CONVERT(VARCHAR(10),APPT_AS_ParRef->AS_Date,120) AS_DateStr
            ,APPT_AS_ParRef->AS_Date
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
            string result = @"
            
           SELECT CONVERT(VARCHAR(10),APPT_AS_ParRef->AS_Date,120) AS_DateStr
            ,APPT_AS_ParRef->AS_Date
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
            string result = @"
            SELECT
            CONVERT(
            VARCHAR(10),
            APPT_AS_ParRef -> AS_Date,
            120
            ) AS_DateStr,
            APPT_AS_ParRef -> AS_Date,
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
            string alg = @"

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
            string algCat = @"

                SELECT ALG_Type_DR->MRCAT_Desc
                FROM PAC_Allergy
                Where ALG_Desc like '{algName}%'

            ";

            algCat = algCat.Replace("{algName}", algName);

            return algCat;
        }

        public static string GetLastEpisode(string hn)
        {
            string result = @"

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
            string result = @"

                select ""JsonData"",""Labjson"" from  ""Admission""  where ""DateAdd"" ='{date}' 

            ";

            result = result.Replace("{date}", date);

            return result;
        }

        public static string GetPatient(string date, string locCode)
        {
            string result = @"

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
            string result = @"

                select ""JsonData"",""Labjson"" from  ""Admission""  where ""HN"" ='{hn}' 

            ";

            result = result.Replace("{hn}", hn);

            return result;
        }

        public static string GetLocation(string site)
        {

            if (site.ToUpper().Trim() == "SNH")
            {
                site = "13";
            }
            else
            {
                site = "12";
            }

            string result = @"

                SELECT CTLOC_Code,CTLOC_Desc
                FROM CT_Loc
                WHERE CTLOC_Hospital_DR = '{site}'  and  CTLOC_DateActiveTo is null
                order by CTLOC_Hospital_DR,CTLOC_Code
            ";

            result = result.Replace("{site}", site);

            return result;
        }

        public static string GetRoom(string buId,string ward)
        {
            string result = @"

                SELECT ROOM_ParRef->WARD_Code,ROOM_Room_DR->ROOM_Code,ROOM_Room_DR->ROOM_Desc
                FROM PAC_WardRoom
                WHERE ROOM_ParRef->WARD_Active = 'Y'
                and left(ROOM_ParRef->WARD_Desc,3)<> 'ZZZ'
                and isnull(ROOM_Room_DR->ROOM_Code,'') not in ('','11FN','12FN') 
                and ROOM_ParRef->WARD_Code not in ('11W7R','12LTC') ";
            if (ward != "%" && ward != "" && ward != null)
            {
                result += "and ROOM_ParRef->WARD_Code = '"+ ward +"' ";
            }else
            {
                result += "and left(ROOM_ParRef->WARD_Code,2) = '" + buId + "' ";
            }
            result += "order by left(ROOM_ParRef->WARD_Code,2),ROOM_Room_DR->ROOM_Code ";
           

            return result;
        }

        public static string GetWard(string buId)
        {
            string result = @"

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
            string result = @"

            select PAPMI_RowId->PAPER_PHOTODocument->docData As ImageHN
            from PA_Patmas where PAPMI_NO ='{hn}'

            ";

            result = result.Replace("{hn}", hn);

            return result;
        }

        public static Tuple<string,Dictionary<string,string>> GetAlertMsg(string hn)
        {
            string result = @"
                SELECT ALM_AlertCategory_DR->ALERTCAT_Code
                ,ALM_AlertCategory_DR->ALERTCAT_Desc
                ,ALM_Message,ALM_Status
                FROM PA_AlertMsg
                WHERE ALM_PAPMI_ParRef->PAPMI_No = ?
                AND ALM_Status = 'A'
            ";

            //result = result.Replace("{hn}", hn);

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("PAPMI_No", hn);

            return new Tuple<string, Dictionary<string, string>>(result,dic);
        }

        public static Tuple<string, Dictionary<string,string>> GetPatientCategory(string hn)
        {
            string result = @"
                SELECT PAPMI_PatCategory_DR->PCAT_Code, PAPMI_PatCategory_DR->PCAT_Desc 
                FROM PA_PATMAS 
                WHERE PAPMI_No = ?
            ";

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("PAPMI_No", hn);

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }

        public static Tuple<string, Dictionary<string, string>> GetLocationByLineBeacon(string beaconId)
        {
            string result = @"
                SELECT ""LineBeacon_CODE"", ""Location_LineBeacon"".""CTLOC_CODE"", ""Location"".""CTLOC_DESC"", ""Location"".""CTLOC_FLOOR""
                FROM ""Location_LineBeacon""
                INNER JOIN ""Location"" ON(""Location_LineBeacon"".""CTLOC_CODE"" = ""Location"".""CTLOC_CODE"")
                WHERE ""LineBeacon_CODE"" = @beaconId
            ";

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("@beaconId", beaconId);

            return new Tuple<string, Dictionary<string, string>>(result, dic);
        }
    }
}