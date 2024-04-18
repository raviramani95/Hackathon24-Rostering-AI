import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AboutComponent } from './about/about.component';
import { ScheduleBuilderComponent } from './schedule-builder/schedule-builder.component';
import { HomeComponent } from './home/home.component';

const routes: Routes = [
  { path: "", component: HomeComponent }, 
  { path: "home", component: HomeComponent }, 
  { path: "schedule-builder", component: ScheduleBuilderComponent },
  { path: "aboutus", component: AboutComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
