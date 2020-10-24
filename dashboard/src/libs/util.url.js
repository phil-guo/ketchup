import globalSetting from "./util.setting";

const requestUrl = {
  //登陆授权
  auth: globalSetting.host + "/auth/token",

  //operates
  getMenuOfOperate: globalSetting.host + "/api/zero/operates/GetMenuOfOperate",
  pageSearchOperate:
    globalSetting.host + "/api/zero/operates/PageSerachOperate",
  operateAddOrEdit:
    globalSetting.host + "/api/zero/operates/CreateOrEditOperate",
  removeOperate: globalSetting.host + "/api/zero/operates/RemoveOperate",

  //menu
  roleMenus: globalSetting.host + "/api/zero/menus/GetMenusByRole",
  pageSearchMenu: globalSetting.host + "/api/zero/menus/PageSerachMenu",
  menuAddOrEdit: globalSetting.host + "/api/zero/menus/CreateOrEditMenu",
  removeMenu: globalSetting.host + "/api/zero/menus/RemoveMenu",
  getMenusSetRole: globalSetting.host + "/api/zero/menus/GetMenusSetRole",

  //sysuser
  pageSerachSysUser:
    globalSetting.host + "/api/zero/sysUsers/PageSerachSysUser",
  createOrEditSysUser:
    globalSetting.host + "/api/zero/sysUsers/CreateOrEditSysUser",
  removeSysUser: globalSetting.host + "/api/zero/sysUsers/RemoveSysUser",

  //role
  pageSearchRole: globalSetting.host + "/api/zero/roles/PageSerachRole",
  removeRole: globalSetting.host + "/api/zero/roles/RemoveRole",
  createOrEditRole: globalSetting.host + "/api/zero/roles/CreateOrEditRole",
  setRolePermission: globalSetting.host + "/api/zero/roles/SetRolePermission"
};

export default requestUrl;
