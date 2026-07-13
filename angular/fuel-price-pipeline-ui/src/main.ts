// import { bootstrapApplication } from '@angular/platform-browser';
// import { appConfig } from './app/app.config';
// import { App } from './app/app';

// bootstrapApplication(App, appConfig)
//   .catch((err) => console.error(err));


import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { FuelTableComponent } from './app/components/fuel-table/fuel-table.component';

bootstrapApplication(FuelTableComponent, {
  providers: [provideHttpClient()]
}).catch(err => console.error(err));
