import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './pages/home/home.component';
import { ProductDetailComponent } from './pages/product-detail/product-detail.component';
import { ProductOverviewComponent } from './pages/product-overview/product-overview.component';
import { NavigationComponent } from './components/navigation/navigation.component';
import { HttpClientModule } from '@angular/common/http';
import {APOLLO_NAMED_OPTIONS, APOLLO_OPTIONS, ApolloModule} from "apollo-angular";
import {HttpLink} from "apollo-angular/http";
import {environment} from "../environments/environment";
import { InMemoryCache } from '@apollo/client/cache';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { CurrencyPipe } from './pipes/currency/currency.pipe';
import { FooterComponent } from './components/footer/footer.component';
import { LoginComponent } from './pages/login/login.component';
import { ToasterComponent } from './components/toaster/toaster.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { OrderOverviewComponent } from './pages/order-overview/order-overview.component';
import { OrderDetailComponent } from './pages/order-detail/order-detail.component';
import { ShoppingCartComponent } from './pages/shopping-cart/shopping-cart.component';
import { PaymentComponent } from './pages/payment/payment.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ProductDetailComponent,
    ProductOverviewComponent,
    NavigationComponent,
    CurrencyPipe,
    FooterComponent,
    LoginComponent,
    ToasterComponent,
    ProfileComponent,
    OrderOverviewComponent,
    OrderDetailComponent,
    ShoppingCartComponent,
    PaymentComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ApolloModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    {
      provide: APOLLO_NAMED_OPTIONS,
      useFactory: (httpLink: HttpLink) => {
        return {
          default: {
            cache: new InMemoryCache(),
            link: httpLink.create({
              uri: environment.urls.catalogue,
            }),
          },
          catalogue: {
            cache: new InMemoryCache(),
            link: httpLink.create({
              uri: environment.urls.catalogue + "graphql",
            }),
          }
        };
      },
      deps: [HttpLink],
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
