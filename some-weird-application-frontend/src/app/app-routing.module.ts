import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DonutChartComponent } from './donut-chart/donut-chart.component';
import { OutputGraphComponent } from './output-graph/output-graph.component';


const routes: Routes = [
  { path: 'domains', component: DonutChartComponent, data: {containerIdentifier: 'donut-chart-websites'} },
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
