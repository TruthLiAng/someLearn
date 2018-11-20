import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TestComponentComponent } from './test/test-component/test-component.component';
import { TestDirectiveDirective } from './test/test-directive.directive';
import { TestPipePipe } from './test/test-pipe.pipe';

@NgModule({
  declarations: [
    AppComponent,
    TestComponentComponent,
    TestDirectiveDirective,
    TestPipePipe
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
