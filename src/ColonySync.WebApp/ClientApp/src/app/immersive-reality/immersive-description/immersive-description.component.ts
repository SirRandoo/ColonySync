import {Component, input} from "@angular/core";

@Component({
  selector: 'app-immersive-description',
  templateUrl: './immersive-description.component.html',
  styleUrl: './immersive-description.component.css'
})
export class ImmersiveDescriptionComponent {
  content = input.required<string>();
}
