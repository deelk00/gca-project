import { ProductDetailComponent } from './pages/product-detail/product-detail.component';
import { HomeComponent } from './pages/home/home.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {ProductOverviewComponent} from "./pages/product-overview/product-overview.component";
import { LoginComponent } from './pages/login/login.component';

const routes: Routes = [
  {
    path: "login",
    component: LoginComponent
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
    component: ProductOverviewComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
