import { ProductDetailComponent } from './pages/product-detail/product-detail.component';
import { HomeComponent } from './pages/home/home.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {ProductOverviewComponent} from "./pages/product-overview/product-overview.component";
import { LoginComponent } from './pages/login/login.component';
import {ProfileComponent} from "./pages/profile/profile.component";
import {OrderOverviewComponent} from "./pages/order-overview/order-overview.component";
import {OrderDetailComponent} from "./pages/order-detail/order-detail.component";

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
    path: "products/:productId",
    component: ProductDetailComponent
  },
  {
    path: "profile/:userId",
    component: ProfileComponent
  },
  {
    path: "orders",
    component: OrderOverviewComponent
  },
  {
    path: "orders/:orderId",
    component: OrderDetailComponent
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
