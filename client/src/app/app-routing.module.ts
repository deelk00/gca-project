import { ProductDetailComponent } from './pages/product-detail/product-detail.component';
import { HomeComponent } from './pages/home/home.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {ProductOverviewComponent} from "./pages/product-overview/product-overview.component";
import {AdminPanelComponent} from "./pages/admin-panel/admin-panel.component";

const routes: Routes = [
  {
    path: "admin",
    component: AdminPanelComponent
  },
  {
    path: "product-category/:id/:id",
    component: ProductOverviewComponent
  },
  {
    path: "product-category/:id",
    component: ProductOverviewComponent
  },
  {
    path: "products/:id",
    component: ProductDetailComponent
  },
  {
    path: "",
    component: HomeComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
