import i18next from "i18next";
import { initReactI18next } from "react-i18next";

import commonTr from "../locales/tr/common.json";
import commonEn from "../locales/en/common.json";
import sidebarTr from "../locales/tr/sidebar.json";
import sidebarEn from "../locales/en/sidebar.json";
import headerTr from "../locales/tr/header.json";
import headerEn from "../locales/en/header.json";
import authTr from "../locales/tr/auth.json";
import authEn from "../locales/en/auth.json";
import crudTr from "../locales/tr/crud.json";
import crudEn from "../locales/en/crud.json";
import companiesTr from "../locales/tr/companies.json";
import companiesEn from "../locales/en/companies.json";
import rolesTr from "../locales/tr/roles.json";
import rolesEn from "../locales/en/roles.json";
import mainRolesTr from "../locales/tr/mainRoles.json";
import mainRolesEn from "../locales/en/mainRoles.json";
import mainRoleRoleRelationshipsTr from "../locales/tr/mainRoleRoleRelationships.json";
import mainRoleRoleRelationshipsEn from "../locales/en/mainRoleRoleRelationships.json";
import mainRoleUserRelationshipsTr from "../locales/tr/mainRoleUserRelationships.json";
import mainRoleUserRelationshipsEn from "../locales/en/mainRoleUserRelationships.json";
import uniformChartOfAccountsTr from "../locales/tr/uniformChartOfAccounts.json";
import uniformChartOfAccountsEn from "../locales/en/uniformChartOfAccounts.json";
import usersTr from "../locales/tr/users.json";
import usersEn from "../locales/en/users.json";
import systemToolsTr from "../locales/tr/systemTools.json";
import systemToolsEn from "../locales/en/systemTools.json";
import selectCompanyTr from "../locales/tr/selectCompany.json";
import selectCompanyEn from "../locales/en/selectCompany.json";
import notFoundTr from "../locales/tr/notFound.json";
import notFoundEn from "../locales/en/notFound.json";
import pageTitlesTr from "../locales/tr/pageTitles.json";
import pageTitlesEn from "../locales/en/pageTitles.json";
import dashboardTr from "../locales/tr/dashboard.json";
import dashboardEn from "../locales/en/dashboard.json";
import profileTr from "../locales/tr/profile.json";
import profileEn from "../locales/en/profile.json";
import notificationsTr from "../locales/tr/notifications.json";
import notificationsEn from "../locales/en/notifications.json";
import userMenuTr from "../locales/tr/userMenu.json";
import userMenuEn from "../locales/en/userMenu.json";
import accountSettingsTr from "../locales/tr/accountSettings.json";
import accountSettingsEn from "../locales/en/accountSettings.json";

const LANGUAGE_KEY = "language";
const savedLanguage = (typeof window !== "undefined" && localStorage.getItem(LANGUAGE_KEY)) || "tr";

i18next.use(initReactI18next).init({
  resources: {
    tr: {
      common: commonTr,
      sidebar: sidebarTr,
      header: headerTr,
      auth: authTr,
      crud: crudTr,
      companies: companiesTr,
      roles: rolesTr,
      mainRoles: mainRolesTr,
      mainRoleRoleRelationships: mainRoleRoleRelationshipsTr,
      mainRoleUserRelationships: mainRoleUserRelationshipsTr,
      uniformChartOfAccounts: uniformChartOfAccountsTr,
      users: usersTr,
      systemTools: systemToolsTr,
      selectCompany: selectCompanyTr,
      notFound: notFoundTr,
      pageTitles: pageTitlesTr,
      dashboard: dashboardTr,
      profile: profileTr,
      notifications: notificationsTr,
      userMenu: userMenuTr,
      accountSettings: accountSettingsTr,
    },
    en: {
      common: commonEn,
      sidebar: sidebarEn,
      header: headerEn,
      auth: authEn,
      crud: crudEn,
      companies: companiesEn,
      roles: rolesEn,
      mainRoles: mainRolesEn,
      mainRoleRoleRelationships: mainRoleRoleRelationshipsEn,
      mainRoleUserRelationships: mainRoleUserRelationshipsEn,
      uniformChartOfAccounts: uniformChartOfAccountsEn,
      users: usersEn,
      systemTools: systemToolsEn,
      selectCompany: selectCompanyEn,
      notFound: notFoundEn,
      pageTitles: pageTitlesEn,
      dashboard: dashboardEn,
      profile: profileEn,
      notifications: notificationsEn,
      userMenu: userMenuEn,
      accountSettings: accountSettingsEn,
    },
  },
  lng: savedLanguage,
  fallbackLng: "tr",
  defaultNS: "common",
  interpolation: { escapeValue: false },
});

export default i18next;
