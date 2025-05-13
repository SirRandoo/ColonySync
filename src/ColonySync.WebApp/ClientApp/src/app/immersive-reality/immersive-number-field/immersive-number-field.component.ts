import {Component, input, model} from "@angular/core";
import {FormsModule} from "@angular/forms";
import {ImmersiveDescriptionComponent} from "../immersive-description/immersive-description.component";
import {ImmersiveFormRowComponent} from "../immersive-form-row/immersive-form-row.component";

@Component({
  selector: 'app-immersive-number-field',
  imports: [
    FormsModule,
    ImmersiveDescriptionComponent,
    ImmersiveFormRowComponent
  ],
  templateUrl: './immersive-number-field.component.html',
  styleUrl: './immersive-number-field.component.css'
})
export class ImmersiveNumberFieldComponent {
  id = input.required<string>();
  label = input.required<string>();
  contents = model<number>();
  placeholder = model<string>();
  min = input<number>(0);
  max = input<number>(99999999);
  step = input<number>();
  description = input<string | undefined>();

  get hasPlaceholder(): boolean {
    let placeholder = this.placeholder;

    return placeholder !== undefined && placeholder.length > 0;
  }

  get hasDescription(): boolean {
    let description: string | undefined = this.description();

    return description !== undefined && description.length > 0;
  }

  protected readonly String = String;
}
