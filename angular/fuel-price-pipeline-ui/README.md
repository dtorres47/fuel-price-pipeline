# fuel-price-pipeline — Angular UI

A deliberately thin web UI for the pipeline — a small full-stack layer, not a frontend showcase. It displays the latest diesel price using the same clean-architecture boundaries as the backends.

See the [root README](../../README.md) for the project overview.

## Structure

```
src/app/
├── domain/                    # DieselFuelPrice interface
├── infra/fuel.service.ts      # data source (currently mocked)
├── usecase/get-latest.service.ts  # wraps the source in a signal
└── components/fuel-table/     # displays the latest rate
```

The layering mirrors the Go and C# backends — domain, a data-access layer, a use case, and a view — kept intentionally minimal.

## Run it

```bash
npm install
npm start        # ng serve → http://localhost:4200
```

## Notes

- **Mock data.** `infra/fuel.service.ts` returns a hardcoded rate with a simulated delay. It's structured to swap for a real HTTP call to the Go API (`GET /getAll` on `:8080`) without touching the use case or component.
- **Scope.** Standalone components and Angular signals, kept small on purpose. The backend is the focus of the project.

## Stack

Angular 20 · TypeScript · RxJS
