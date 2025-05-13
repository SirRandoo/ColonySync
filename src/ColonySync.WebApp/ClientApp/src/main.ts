import {enableProdMode} from "@angular/core";
import {environment} from "./environments/environment";
import {bootstrapApplication} from "@angular/platform-browser";
import {AppComponent} from "./app/app.component";
import {provideRouter, Routes} from "@angular/router";
import {SettingsComponent} from "./app/settings/settings.component";

export function getBaseUrl() {
  return document.getElementsByTagName("base")[0].href;
}

const providers = [
  {provide: "BASE_URL", useFactory: getBaseUrl, deps: []}
];

const routes: Routes = [
  {path: 'settings', component: SettingsComponent}
]

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {providers: [provideRouter(routes), ...providers]}).catch(e => console.error(e));
