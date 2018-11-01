import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';

const routes: Routes = [
  {path: '', redirectTo: 'design', pathMatch: 'full'},
  {
    path: '',
    children: [
      {
        path: 'design',
        loadChildren: './query-generator/query-generator.module#QueryGeneratorModule'
      }
    ]
  }
]


@NgModule({
  imports: [RouterModule.forRoot(routes, {useHash: true})],
  exports: [RouterModule]
})
export class AppRoutingModule {
}