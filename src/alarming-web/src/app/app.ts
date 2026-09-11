import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { MatButtonModule } from "@angular/material/button";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";

@Component({
    selector: "app-root",
    imports: [RouterOutlet, MatButtonModule, MatFormFieldModule, MatInputModule],
    templateUrl: "./app.html",
    styleUrl: "./app.scss"
})
export class App {
    protected readonly title = "Alarming";
}
