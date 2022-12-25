SET SERVEROUTPUT ON
--BEGIN
--  update sys_code_openid set ngay = TO_NUMBER(to_char(sysdate, 'yyyyMMdd')||replace(to_char(systimestamp, 'HH24'),'.','')); commit;
--END;

DECLARE
 --------------------------------
 b_env VARCHAR2(50):= 'PRODUCTION';
 b_server_ip VARCHAR2(50):='27.71.231.47';
 b_server_port VARCHAR2(50):='1521';
 b_use_sid NUMBER:= 0; -- 1 Dung SERVICENAME, 0 dung SID
 b_database VARCHAR2(50):='escs';
 b_schema VARCHAR2(50):='escs_product';
 b_username VARCHAR2(50):='escs_product';
 b_password VARCHAR2(50):='escs1234';
 b_db_cache VARCHAR2(50):='1';
 b_ngay_ht  NUMBER;
 --------------------------------
 b_webserver_ip VARCHAR2(50):='27.71.231.47';
 b_ung_dung_ten NVARCHAR2(200):= N'H? TH?NG QU?N LÝ B?I TH??NG ESCS';
 
 b_ung_dung_web VARCHAR2(50):='ESCS_CLAIM_CMS';
 b_ung_dung_web_token VARCHAR2(100):='da0d30a89d209692b74b792882ce6dad';
 b_ung_dung_web_secret VARCHAR2(100):='c5212904ad7856b885f589e30011fe20';
 
 b_ung_dung_mobile VARCHAR2(50):='ESCS_CLAIM_MOBILE_APP';
 b_ung_dung_mobile_secret VARCHAR2(200):='568c85c6c8c5e64ee71b098028a4d7a7';
 b_ung_dung_mobile_user VARCHAR2(100):='escs_mobile';
 b_ung_dung_mobile_pas VARCHAR2(200):='8557d3e459444a04a923462f953d18bd1c359e7b9b9c9e478c66ec45f61d4b87';
 
 b_cms_openapi_user VARCHAR2(50):='openid_cms';
 b_cms_openapi_pas_sha256 VARCHAR2(200):='555ef214fab435af1b67960b3374d99619e5d94574f4a87cdd3f7db17e6f13ce';
 --------------------------------
 b_db_id  NUMBER;
 b_schema_id  NUMBER;
 b_webserver_id  NUMBER;
BEGIN
  --create or replace NONEDITIONABLE TYPE "SPLIT_TBL" as table of varchar2(1000);
  --Compile lai function
  --b_ngay_ht := TO_NUMBER(to_char(sysdate, 'yyyyMMdd')||replace(to_char(systimestamp, 'HH24'),'.',''));
  --update sys_code_openid set ngay = b_ngay_ht; commit;
  DBMS_OUTPUT.PUT_LINE('sys_database');
  delete from sys_database t where t.dbname = b_database;
  b_db_id:= PKG_SYS_COMMON.FUNC_CREATE_CODE();
  INSERT INTO sys_database (id,dbname,descriptions,cat_db,createdate,createby,updatedate,updateby,isactive,set_default) 
  VALUES (b_db_id,b_database,'','ORACLE',func_convert_date_to_num(sysdate),'system', null, null,1,1);
  DBMS_OUTPUT.PUT_LINE('sys_database_config');
  delete from sys_database_config t where t.database_id = b_db_id;
  INSERT INTO sys_database_config (id,database_id,dbname,servername,port,connectstring,sid,service_name,use_sid,
    envcode,createdate,createby,updatedate,updateby,isactive)
  VALUES (PKG_SYS_COMMON.FUNC_CREATE_CODE(),b_db_id,b_database,b_server_ip,b_server_port,null,b_database,b_database,b_use_sid,
    b_env,func_convert_date_to_num(sysdate),'system', null, null,1);
  DBMS_OUTPUT.PUT_LINE('sys_schema');
  delete from sys_schema t where t.schema = b_schema;
  b_schema_id:= PKG_SYS_COMMON.FUNC_CREATE_CODE();
  INSERT INTO sys_schema (id,database_id,schema,descriptions,createdate,createby,updatedate,updateby,isactive,set_default) 
  VALUES (b_schema_id,b_db_id,b_schema,'',func_convert_date_to_num(sysdate),'system',null,null,1,1);
  DBMS_OUTPUT.PUT_LINE('sys_schema_config');
  delete from sys_schema_config t where t.schema_id=b_schema_id;
  INSERT INTO sys_schema_config (id,schema_id,envcode,username,password,createdate,createby,updatedate,updateby,isactive) 
  VALUES (PKG_SYS_COMMON.FUNC_CREATE_CODE(), b_schema_id,b_env,b_username,b_password,
  func_convert_date_to_num(sysdate),'system',null,null,1);
  DBMS_OUTPUT.PUT_LINE('sys_partner');
  delete from sys_partner t where t.code = b_ung_dung_web;
  delete from sys_partner t where t.code = b_ung_dung_mobile;
  INSERT INTO sys_partner (code,name,parent_code,organization,companyname,ceo_name,address,
    email,phone,taxcode,contractno,description,website,cat_partner,effectivedate,expirationdate,
    createdate,createby,updatedate,updateby,isactive,ismaster,action_login) 
  VALUES (b_ung_dung_web,b_ung_dung_ten,null,'ORGANIZATION',null,null,null,null,null,null,null,null,null,'PRIVATE',
  20210101,21500101,func_convert_date_to_num(sysdate),'system',null,null,1,0, null);
  INSERT INTO sys_partner (code,name,parent_code,organization,companyname,ceo_name,address,
    email,phone,taxcode,contractno,description,website,cat_partner,effectivedate,expirationdate,
    createdate,createby,updatedate,updateby,isactive,ismaster,action_login) 
  VALUES (b_ung_dung_mobile,b_ung_dung_ten||' (MOBILE)',null,'ORGANIZATION',null,null,null,null,null,null,null,null,null,'PUBLIC',
  20210101,21500101,func_convert_date_to_num(sysdate),'system',null,null,1,0, 'PY8AHDD1UZA71X0');
  DBMS_OUTPUT.PUT_LINE('sys_partner_config');
  delete from sys_partner_config t where t.partner_code = b_ung_dung_web;
  delete from sys_partner_config t where t.partner_code = b_ung_dung_mobile;
  INSERT INTO sys_partner_config (id,partner_code,envcode,ip_cors,token,secret_key,password,
    createdate,createby,updatedate,updateby,isactive,username,session_time_live,username_cms,
    password_cms,blacklist_ip) 
  VALUES (PKG_SYS_COMMON.FUNC_CREATE_CODE(),b_ung_dung_web,b_env,null,b_ung_dung_web_token,b_ung_dung_web_secret,null,
    func_convert_date_to_num(sysdate),'system',null,null,1,null,120,b_cms_openapi_user,b_cms_openapi_pas_sha256,null);
  INSERT INTO sys_partner_config (id,partner_code,envcode,ip_cors,token,secret_key,password,
    createdate,createby,updatedate,updateby,isactive,username,session_time_live,username_cms,
    password_cms,blacklist_ip) 
  VALUES (PKG_SYS_COMMON.FUNC_CREATE_CODE(),b_ung_dung_mobile,b_env,null,'',b_ung_dung_mobile_secret,b_ung_dung_mobile_pas,
    func_convert_date_to_num(sysdate),'system',null,null,1,b_ung_dung_mobile_user,120,null,null,null);
  DBMS_OUTPUT.PUT_LINE('sys_server');
  delete from sys_server;
  b_webserver_id:=PKG_SYS_COMMON.FUNC_CREATE_CODE();
  INSERT INTO sys_server (id,servername,cat_server,createdate,createby,updatedate,updateby,isactive,set_default) 
  VALUES (b_webserver_id,'Web Server','DB_CACHE_FILES',func_convert_date_to_num(sysdate),'system',null,null,1,1);
  DBMS_OUTPUT.PUT_LINE('sys_server_config');
  delete from sys_server_config t where t.server_id = b_webserver_id;
  INSERT INTO sys_server_config (id,server_id,envcode,server_ip,createdate,createby,updatedate,updateby,isactive,setdefault) 
  VALUES (PKG_SYS_COMMON.FUNC_CREATE_CODE(),b_webserver_id,b_env,b_webserver_ip,
  func_convert_date_to_num(sysdate),'system',null,null,1,null);
  
  update sys_action_exc_db t set t.schema_id = b_schema_id;
  update sys_action_file t set t.schema_id = b_schema_id;
  update sys_action_mail t set t.schema_id = b_schema_id;
  update sys_action_config t set t.id_server_cache= b_webserver_id, t.envcode = b_env;
  
  update sys_action_config t set t.db_cache=b_db_cache;
  commit;
  DBMS_OUTPUT.PUT_LINE('Thanh Cong');
END;


--select PKG_SYS_COMMON.FUNC_CREATE_CODE() as code from dual;
--select * from sys_action_config a;
--select * from sys_database a;
--select * from sys_database_config b;
--select * from sys_schema c;
--select * from sys_schema_config;
--select * from sys_server;
--select * from sys_server_config;
--select * from sys_partner;
--select * from sys_partner_config;
--select * from sys_server;
--select * from sys_server_config;