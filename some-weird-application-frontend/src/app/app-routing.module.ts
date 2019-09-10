import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OutputGraphComponent } from './output-graph/output-graph.component';
import { SiteGraphComponent } from './site-graph/site-graph.component';


const routes: Routes = [
  { path: 'domains', component: SiteGraphComponent, data: {containerIdentifier: 'donut-chart-websites'} },
  { path: 'default', component: OutputGraphComponent },
  { path: '',
    redirectTo: '/default',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
