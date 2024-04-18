import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FullCalendarModule } from '@fullcalendar/angular';
import { ScheduleBuilderComponent } from './schedule-builder/schedule-builder.component';
import { AboutComponent } from './about/about.component';
import { HomeComponent } from './home/home.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { HeaderComponent } from './header/header.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
// Import library module
import { NgxSpinnerModule } from "ngx-spinner";

@NgModule({
  declarations: [
    AppComponent,
    ScheduleBuilderComponent,
    AboutComponent,
    HomeComponent,
    SidebarComponent,
    HeaderComponent    
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FullCalendarModule,
    BrowserAnimationsModule,
    NgSelectModule,
    FormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    NgxSpinnerModule.forRoot({ type: 'ball-scale-ripple-multiple' }),
  ],
  providers: [],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
