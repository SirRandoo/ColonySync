import {Component, input, model} from "@angular/core";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ImmersiveDescriptionComponent} from "../immersive-description/immersive-description.component";
import {ImmersiveFormRowComponent} from "../immersive-form-row/immersive-form-row.component";

@Component({
  selector: "app-immersive-checkbox",
  imports: [
    ReactiveFormsModule,
    FormsModule,
    ImmersiveFormRowComponent,
    ImmersiveDescriptionComponent
  ],
  templateUrl: "./immersive-checkbox.component.html",
  styleUrl: "./immersive-checkbox.component.css"
})
export class ImmersiveCheckboxComponent {
  id = input.required<string>();
  label = input.required<string>();
  checked = model<boolean>(false);
  description = model<string | undefined>();

  toggle() {
    this.checked.set(!this.checked());
  }

  get hasDescription(): boolean {
    let description: string | undefined = this.description();

    return description !== undefined && description.length > 0;
  }

  protected readonly String = String;
}
