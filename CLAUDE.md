# CLAUDE.md

Guidance for Claude Code (and other AI assistants) working in this repository.

## Project

FamilyCalender — a family calendar & shared-list web app.

- **Stack:** .NET 8, ASP.NET Core **Razor Pages**, Bootstrap 5.3, EF Core.
- **Projects:**
  - `FamilyCalender.Core` — domain models / business logic
  - `FamilyCalender.Infrastructure` — data access (EF Core) & services
  - `FamilyCalender.Web` — Razor Pages UI (`Pages/`, `wwwroot/`)
  - `FamilyCalender.Tests`, `FamilyCalendar.Infrastructure.Tests` — tests

## Common commands

```bash
dotnet build FamilyCalender.sln          # build everything
dotnet run --project FamilyCalender.Web  # run the web app
dotnet test                              # run all tests
```

## How I want you to work

- **Svara på svenska** (konversation och förklaringar).
- **Kommentarer i koden skrivs på svenska** (matchar befintlig stil); kod/identifierare på engelska.
- Be concise; explain the reasoning behind non-trivial decisions, not just the *what*.
- Prefer editing existing files and matching the surrounding style over introducing new patterns.
- For larger or hard-to-reverse changes, plan first and confirm before executing.
- **Never push or open PRs unless I explicitly ask.** Commit only when I ask.
- After changing code, verify with `dotnet build` / `dotnet test` when possible.

## Git & commits

- Do feature work on a branch off `main`.
- **Commit messages: English, Conventional Commits format.**

  ```
  type(scope): short imperative summary
  ```

  - **type:** `feat` | `fix` | `refactor` | `style` | `docs` | `test` | `chore` | `build`
  - **scope** (optional): area touched — e.g. `css`, `calendar`, `listpage`, `auth`, `web`, `infrastructure`, `db`
  - **summary:** imperative mood, lowercase, no trailing period, ≤ ~72 chars
  - Optional body after a blank line: `-` bullets explaining *why* / notable details.

  Examples:
  ```
  refactor(css): remove dead theme, fix encoding, dedupe password toggle
  fix(listpage): correct singular/plural weekdays for day intervals
  feat(checklist): add reverse-list button
  ```

### Local git identity (WSL quirk)

`git config` can't be written on this WSL mount (`chmod on .git/config.lock: Operation not permitted`) and no identity is set, so a plain `git commit` aborts with "Author identity unknown". Commit with the identity inline:

```bash
git -c user.name="robin" -c user.email="robin.liliegren@outlook.com" commit -m "..."
```

## CSS architecture (Razor Pages)

Stylesheets load in this order (see `_Layout.cshtml` / `_LayoutPublic.cshtml`):

1. `css-variables.css` — design tokens (`--pmf-*`)
2. `bootstrap-overrides.css` — Bootstrap customisation, PMF buttons
3. `site.css` — global app styles (loaded by **both** layouts)
4. `public.css` — public/landing & auth theme (only `_LayoutPublic`)
5. `*.cshtml.css` — Razor CSS-isolation, page-specific only

Rules:
- Page-specific styles live in that page's `.cshtml.css`. A style shared across pages goes in `site.css` (not duplicated per page).
- All CSS files are **UTF-8**.
