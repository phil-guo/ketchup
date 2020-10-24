import { uniqueId } from "lodash";
import util from "@/libs/util.js";
import { date } from "faker";
import $ from "jquery";
// import { reject, resolve } from "core-js/fn/promise";

/**
 * @description 给菜单数据补充上 path 字段
 * @description https://github.com/d2-projects/d2-admin/issues/209
 * @param {Array} menu 原始的菜单数据
 */
function supplementPath(menu) {
  return menu.map(e => ({
    ...e,
    path: e.path || uniqueId("d2-menu-empty-"),
    ...(e.children
      ? {
          children: supplementPath(e.children)
        }
      : {})
  }));
}

function supplementAsidePath() {
  let menus = getRoleMenus(); 
  return menus.map(e => ({
    ...e,
    path: e.path || uniqueId("d2-menu-empty-"),
    ...(e.children
      ? {
          children: supplementPath(e.children)
        }
      : {})
  }));
}
function supplementHeadPath(){
  let menus = getRoleMenus(); 
  return menus.map(e => ({
    ...e,
    path: e.path || uniqueId("d2-menu-empty-"),
    ...(e.children
      ? {
          children: supplementPath(e.children)
        }
      : {})
  }));
}

function getRoleMenus() {
  let menus = [];
  // $.ajax({
  //   url: util.requestUrl.roleMenus,
  //   type: "post",
  //   contentType: "application/json",
  //   async: false,
  //   data: JSON.stringify({
  //     roleId: util.cookies.get(util.globalSetting.roleId)
  //   }),
  //   success: function(response) {
  //     menus = response.result.datas;
  //   }
  // });

  return menus;
}

export const menuHeader = supplementHeadPath();

export const menuAside = supplementAsidePath();
