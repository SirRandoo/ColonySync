import {Component} from "@angular/core";
import {RouterLink, RouterLinkActive} from "@angular/router";

@Component({
  selector: "app-nav-menu",
  templateUrl: "./nav-menu.component.html",
  imports: [
    RouterLink,
    RouterLinkActive
  ],
  styleUrls: ["./nav-menu.component.css"]
})
export class NavMenuComponent {
}
