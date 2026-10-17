[forgeerp-design-system.md](https://github.com/user-attachments/files/33541857/forgeerp-design-system.md)
# ForgeERP Design System & Frontend Handoff

**Author:** Mahmoud Ragab  
**Platform:** React 19, TypeScript, Vite, Tailwind CSS v4  
**Source of truth reviewed:** `src/index.css`, `src/design-system.tsx`, `src/App.tsx`, and the Products, Inventory, Sales, and Purchasing page implementations.

> **Verification key:** “Verified” means the value or behavior is directly present in the inspected source. “Needs verification in Figma” means the current source does not establish an exact design value or supported behavior.

## Product-wide rules

- The logged-in user is **Mahmoud Ragab**.
- Preserve the existing screens; this handoff documents rather than redesigns them.
- Forge AI may analyze data and propose actions.
- Every AI-proposed write action must require explicit human approval through `ApprovalModal`.
- No purchase order is executed before the user selects **Approve**.

---

## 1. Design tokens

### Color tokens

| Token name | Actual value | CSS variable suggestion | Where it is used | Status |
|---|---:|---|---|---|
| Primary | `#2563EB` | `--color-primary` | Primary actions, links, sorting, actual chart series | Verified |
| Primary hover | `#1D4ED8` | `--color-primary-hover` | Primary button hover | Verified |
| Primary soft | `#EFF6FF` | `--color-primary-soft` | Product icons, user avatars | Verified |
| AI accent | `#6D5BD0` | `--color-ai` | AI actions, confidence, forecasts, AI labels | Verified |
| AI soft | `#F2EFFF` | `--color-ai-soft` | AI notices, recommendation surfaces | Verified |
| Sidebar | `#101827` | `--color-sidebar` | Fixed application sidebar | Verified |
| Sidebar muted | `#9BA7BA` | `--color-sidebar-muted` | Sidebar secondary text | Verified |
| Background | `#F6F7F9` | `--color-background` | Application canvas, subtle fills | Verified |
| Surface / card | `#FFFFFF` | `--color-surface` | Cards, panels, menus, modals, fields | Verified |
| Border | `#E2E6EC` | `--color-border` | Cards, tables and dividers | Verified |
| Border strong | `#CDD3DC` | `--color-border-strong` | Inputs, selects, secondary buttons | Verified |
| Text | `#172033` | `--color-text` | Titles, values, emphasized text | Verified |
| Text secondary | `#5E6879` | `--color-text-secondary` | Body copy and secondary labels | Verified |
| Text muted | `#8B94A3` | `--color-text-muted` | Captions, metadata and placeholders | Verified |
| Success | `#15803D` | `--color-success` | Positive trends and success status | Verified |
| Success soft | `#ECFDF3` | `--color-success-soft` | Success badges and icon backgrounds | Verified |
| Warning | `#B45309` | `--color-warning` | Low stock, pending approval, warning trends | Verified |
| Warning soft | `#FFF7ED` | `--color-warning-soft` | Warning badges and icon backgrounds | Verified |
| Error / danger | `#C2413B` | `--color-danger` | Stockout risk, rejection, destructive actions | Verified |
| Error / danger soft | `#FEF2F2` | `--color-danger-soft` | Risk badges and warning surfaces | Verified |
| Information | `#0369A1` | `--color-info` | Informational status and neutral metrics | Verified |
| Information soft | `#F0F9FF` | `--color-info-soft` | Informational badges and icons | Verified |
| Standalone secondary brand | Needs verification in Figma | Not defined | No standalone secondary token exists in source | Needs verification in Figma |

Additional literal colors exist for sidebar states, AI borders and subtle table backgrounds. Promote them to semantic variables before reusing them broadly.

### Typography

| Token / role | Actual value | CSS variable suggestion | Usage | Status |
|---|---|---|---|---|
| Font family | Inter 400, 500, 600, 700; `sans-serif` fallback | `--font-family-sans` | Entire application | Verified |
| Display / page title | 25px, 700, 1.25 line-height | `--font-display` | Page titles and dashboard greeting | Verified |
| Overview metric value | 23px, 700, normal line-height | `--font-metric` | KPI value | Verified |
| Modal title / title token | 15px, 700 | `--font-title` | Modal titles | Verified |
| Card title | 13px, 700 | `--font-card-title` | Card headings | Verified |
| Body / controls | 12px, 400–600 | `--font-body` | Buttons, inputs, descriptions | Verified |
| Small body | 10px, 400–700 | `--font-body-sm` | Table body, chat, product labels | Verified |
| Caption | 9px, 400–700 | `--font-caption` | Badges, metadata, helper text | Verified |
| Micro / table header | 8px, 600 | `--font-micro` | Table headers, chart axes | Verified |
| Detailed line-height scale | Needs verification in Figma | — | Most source styles rely on normal line-height | Needs verification in Figma |

### Spacing, radii, borders and elevation

| Token name | Actual value | CSS variable suggestion | Usage | Status |
|---|---:|---|---|---|
| Space 1 | 4px | `--space-1` | Tight metadata gaps | Verified |
| Space 2 | 8px | `--space-2` | Controls and action gaps | Verified |
| Space 3 | 12px | `--space-3` | Compact internal padding | Verified |
| Space 4 | 16px | `--space-4` | Standard component rhythm | Verified |
| Space 5 | 20px | `--space-5` | Modal and content groups | Verified |
| Space 6 | 24px | `--space-6` | Page heading and sidebar rhythm | Verified |
| Space 8 | 32px | `--space-8` | Desktop content padding | Verified |
| Radius small | 6px | `--radius-sm` | Buttons, fields, compact callouts | Verified |
| Radius medium | 10px | `--radius-md` | Menus, notices, grouped fields | Verified |
| Radius large | 14px | `--radius-lg` | Cards and modal | Verified |
| Standard border | 1px | — | Cards, controls, tables and dividers | Verified |
| Shadow small | `0 1px 2px rgba(16,24,40,.04)` | `--shadow-sm` | Cards and active tabs | Verified |
| Shadow medium | `0 8px 24px rgba(16,24,40,.10)` | `--shadow-md` | Dropdown menus | Verified |
| Shadow large | `0 20px 48px rgba(16,24,40,.16)` | `--shadow-lg` | Assistant and modal | Verified |

### Iconography

- Custom inline SVG icon system through `Icon`.
- Default size: **16 × 16px**; medium size: **20 × 20px**.
- Stroke: **1.8px**, round caps and joins, no fill.
- Icons inherit the semantic text color of their parent.
- Use icon-only buttons with an explicit `aria-label`.

---

## 2. Layout specifications

| Area | Specification | Status |
|---|---|---|
| Sidebar | Fixed left; 248px wide; padding 24px 14px 16px | Verified |
| Sidebar navigation item | 38px minimum height; 11px horizontal padding | Verified |
| Top navigation | 66px high; 32px horizontal padding | Verified |
| Main offset | 248px left margin | Verified |
| Main content | 1440px maximum width; centered; 28px 32px 48px padding | Verified |
| Minimum viewport | 1180px body minimum width | Verified |
| Global search | 350px wide; 36px high | Verified |
| KPI grid | Four equal columns; 14px gap | Verified |
| KPI card | 18px padding; 15px at viewport widths ≤1250px | Verified |
| Dashboard / sales grid | `1.65fr minmax(350px, 1fr)`; 14px gap | Verified |
| Purchasing AI grid | `1.55fr minmax(350px, 1fr)`; 14px gap | Verified |
| Inventory insight grid | Three equal columns | Verified |
| Standard control | 36px minimum height | Verified |
| Badge | 22px minimum height; 8px horizontal padding | Verified |
| Table header cell | 10px 14px padding | Verified |
| Table body cell | 11px 14px padding | Verified |
| Table exact row height | Content-driven; skeleton rows are 58px | Needs verification in Figma |
| Assistant | 420px wide; fixed to right; full viewport height | Verified |
| Approval modal | 570px wide; maximum height 92vh | Verified |
| Mobile behavior | No mobile reflow is implemented | Needs verification in Figma |

### Supported responsive behavior

- At viewport widths at or below **1250px**, content horizontal padding changes from 32px to 22px.
- At the same breakpoint, overview KPI card padding changes from 18px to 15px and KPI value size changes from 23px to 20px.
- The body remains at least 1180px wide.
- Do not infer mobile navigation, stacked grids, or compact tables until verified in Figma.

---

## 3. Page-by-page specifications

### 3.1 Overview / Dashboard

**Purpose:** Summarize pharmacy performance and prioritize operational attention.

**Hierarchy**
1. Date eyebrow, personalized greeting for Mahmoud Ragab, operational badge, description.
2. Date range, export and primary transaction actions.
3. Four `InsightCard` KPIs.
4. Sales forecast and Needs attention cards in a split grid.
5. Inventory health table.

**Key components and behavior**
- KPI cards show label, value, trend, comparison label and a semantic icon.
- Forecast combines blue actual series, purple dashed AI forecast, a “Today” marker and 92% confidence.
- Needs attention contains severity icons, business impact metadata and a contextual action.
- “Review order” opens `ApprovalModal`.
- Inventory tabs are visible; their current sample callback does not change data.
- Export opens a dropdown with CSV, PDF and scheduled report options.
- Approval success appears as a dismissible green banner.

**States**
- Success banner: implemented.
- Loading, empty and error states for the Overview inventory table: Needs verification in Figma.

### 3.2 Products

**Purpose:** Manage catalog, pricing, suppliers and inventory levels.

**Hierarchy**
1. Page heading and Add product action.
2. Full-width AI insight card.
3. Catalog card with filters, active result count, table and pagination.
4. Add product modal.

**Behavior**
- Search matches product name, SKU or supplier.
- Category and stock status filters update the table and reset pagination.
- All displayed data columns are sortable.
- Five rows per page.
- Initial table skeleton lasts 350ms in the prototype.
- No-results state offers Clear all filters.
- Row actions open a dropdown.

**States:** loading skeleton, empty search/filter result, disabled pagination and modal are implemented. Dedicated error and save-success states need verification in Figma.

### 3.3 Inventory

**Purpose:** Monitor availability, demand forecasts and replenishment requirements across locations.

**Hierarchy**
1. Heading and Adjust stock action.
2. Four KPI cards.
3. AI inventory intelligence card with three equal insights.
4. Inventory table.

**Behavior**
- Search by product or SKU.
- Filter by category, stock status and location.
- Sort all operational columns.
- `RiskIndicator` maps low to success, medium to warning and high to danger.
- Stock status uses semantic badges.
- Five rows per page with skeleton and empty states.

**Responsive behavior:** No inventory-specific responsive design beyond the shared ≤1250px adjustment.

### 3.4 Sales

**Purpose:** Monitor transactions, payment activity and demand signals.

**Hierarchy**
1. Heading, date range and New sale action.
2. Four sales KPI cards.
3. Forecast and sales intelligence split.
4. Transaction table.
5. New sale modal.

**Behavior**
- Search by invoice or customer.
- Filter by payment and sales status.
- Sort transaction columns; date defaults descending.
- Payment and transaction statuses use semantic badges.
- New sale modal supports customer, location and product search fields.
- Empty product composition area appears before products are added.

**States:** table skeleton, no-results empty state, disabled pagination and new-sale empty composition are implemented. Checkout loading/error/success need verification in Figma.

### 3.5 Purchasing

**Purpose:** Manage purchase orders, supplier fulfillment and AI-assisted replenishment.

**Hierarchy**
1. Heading and New purchase order action.
2. Four purchasing KPI cards.
3. AI purchase recommendation and purchasing intelligence split.
4. Purchase order table.
5. New order and approval modals.

**Behavior**
- AI recommendation displays priority, product risk, projected stockout, quantity, supplier, total and delivery.
- Review recommendation opens `ApprovalModal`.
- Approve closes the modal and displays a dismissible success banner.
- Search by PO number or supplier; filter by order status, supplier and date range.
- Sort purchase order columns and paginate five rows at a time.
- Status and approval status are separate semantic badges.

**Human approval:** The recommendation explicitly states that no order is created without approval.

### 3.6 AI Assistant

**Purpose:** Explain business signals and turn recommendations into reviewable actions.

**Layout**
- Fixed right panel, 420px wide.
- 68px header with Forge AI identity, ready state and close action.
- Purple safety context strip.
- Scrollable conversation.
- Composer anchored at the bottom.

**Conversation components**
- `UserMessage`: blue bubble, right aligned, MR avatar.
- `AIMessage`: neutral bubble, left aligned, Forge AI avatar.
- `SourceList`: bordered source block with checked source chips.
- Risk summary: High risk badge plus estimated revenue impact.
- Recommendation card: order, reason, supplier and estimated total.
- `ActionButton`: AI-colored, shield icon, opens approval review.

**Behavior**
- Opens from Ask Forge AI in the top navigation.
- Slides in with a 240ms transition and page scrim.
- Scrim and close button dismiss it.
- Current send button has no implemented submit behavior.
- Assistant typing, loading, error and retry states need verification in Figma.

### 3.7 AI Action Approval Modal

**Purpose:** Require explicit human review before executing an AI-proposed write action.

**Layout**
- 570px centered modal over dark scrim.
- Header: AI icon, “AI-proposed action” eyebrow, title and close.
- Body: control notice, action type, product/risk, detail grid, AI reasoning and risk of inaction.
- Footer: Cancel, Reject and Approve.

**Behavior**
- `onApprove` is the only callback that executes the approved action.
- Reject invokes `onReject` when provided, otherwise closes.
- Cancel and close dismiss without approval.
- Focus trapping, escape-key behavior, approval loading, failure and retry behavior need verification in Figma.

---

## 4. Reusable component inventory

| Component | Variants / properties | Visual specification | Expected behavior |
|---|---|---|---|
| `Button` | primary, secondary, ghost, danger, AI, icon-only; optional icon | 36px minimum height, 6px radius, 12px/600 text | Hover states; disabled pagination at 45% opacity |
| `Input` | icon, suffix plus native input props | 36px, strong border, 6px radius | Primary focus border and 3px soft ring |
| `Select` | options plus native select props | 36px, trailing chevron | Native selection |
| `Dropdown` | trigger, items | 170px menu, 10px radius, medium shadow | Trigger toggles; item closes menu |
| `InsightCard` | label, value, trend, trendLabel, icon, neutral/positive/warning | Card with top-right semantic icon | Present KPI and comparison |
| Data table | table primitives, sortable button composition | Uppercase 8px headers, 10px body, row hover | Horizontal overflow, sorting, pagination |
| `Badge` | neutral, success, warning, danger, info, AI; optional dot | 22px pill, 9px/600 | Text must accompany semantic color |
| `RiskIndicator` | low, medium, high | Low/success, medium/warning, high/danger | Display explicit risk word |
| `ForgeSidebar` | active, onChange, optional sections | 248px dark fixed rail | Changes active page |
| `InsightCard` | KPI data and variant | Semantic icon and trend | Read-only |
| `ForecastChart` | no public props in current source | Blue actual, purple dashed forecast | Static SVG in current implementation |
| `AssistantChatPanel` | open, onClose, onApprove | 420px slide-in panel | Dismiss or hand recommendation to approval |
| `ApprovalModal` | open, onClose, onApprove, onReject?, estimatedTotal? | 570px modal with 3-action footer | Explicit gate before action execution |

---

## 5. AI component specifications

### Insights and risk

- AI identity uses the purple AI token and sparkle icon.
- Insight cards pair a semantic symbol with a concise title, explanation and metadata.
- High risk uses danger; medium uses warning; low uses success.
- Risk is always written as text and is not communicated by color alone.
- Business impact should be concrete where known: timeframe, units, revenue or affected orders.

### Forecasts and confidence

- Actual/historical series: primary blue, solid, 2.5px.
- AI forecast: purple, dashed, 2.5px with a `6 5` dash pattern.
- Confidence is written explicitly with a sparkle icon; current screens show 92%.
- The current chart is static; dynamic chart behavior and tooltips need verification in Figma.

### Assistant responses and sources

- AI responses use neutral bubbles and may include paragraphs, semantic risk, sources and a recommendation card.
- `SourceList` is titled “Sources / Data used” and shows checked source chips.
- Do not invent API response fields from these visual components.

### Recommendation to approval flow

1. AI explains evidence and recommendation.
2. UI states that the recommendation does not execute automatically.
3. User selects Review purchase order / Review recommendation.
4. `ApprovalModal` shows action details, reasoning and risk.
5. User explicitly chooses Cancel, Reject or Approve.
6. Success feedback appears only after approval.

### AI and approval states

| State | Existing presentation | Status |
|---|---|---|
| Recommendation | Purple AI badge/card with evidence and action | Verified |
| Approval required | Shield notice and explicit review action | Verified |
| Approved | Modal closes; green success banner | Verified |
| Rejected | Modal closes through reject callback/fallback | Verified |
| AI loading / typing | Not present | Needs verification in Figma |
| Approval pending | Not present | Needs verification in Figma |
| AI / approval error | Not present | Needs verification in Figma |
| Retry | Not present | Needs verification in Figma |

---

## 6. Frontend implementation guidance

### Suggested verified CSS variables

Continue using the existing `--color-*`, `--space-*`, `--radius-*`, `--shadow-*` and `--font-*` variables. Add new semantic aliases only when a repeated role is established; do not create one-off color variables by visual resemblance.

### Suggested component names

- `AppShell`, `ForgeSidebar`, `Topbar`
- `PageHeading`, `Card`, `CardHeader`
- `Button`, `Input`, `Select`, `Dropdown`, `Tabs`
- `InsightCard`, `RiskIndicator`, `RecommendationCard`
- `DataTable`, `Pagination`, `EmptyState`, `TableSkeleton`
- `ForecastChart`
- `AssistantChatPanel`, `UserMessage`, `AIMessage`, `SourceList`, `ActionButton`
- `Modal`, `ApprovalModal`

### Naming conventions

- Components: PascalCase.
- Props and state: camelCase.
- CSS custom properties: `--category-role-state`.
- Semantic colors: primary, AI, success, warning, danger, info, text, border, surface, background.
- Spacing: numeric scale tied to 4px increments.
- Radii and elevation: `sm`, `md`, `lg`.

### Accessibility

- Preserve `aria-label` on icon-only controls and table action headers.
- Keep modal `role="dialog"`, `aria-modal`, and labelled title.
- Add focus trapping and focus return before production; current source does not implement them.
- Add Escape behavior after confirming the intended modal/panel interaction.
- Maintain text labels with all status colors.
- Verify contrast of muted 8–9px text in production.
- Respect reduced-motion preferences for panel and skeleton animation; current source needs enhancement.
- Ensure sortable headers communicate sort state with `aria-sort`; current source uses labels but not `aria-sort`.

### Developer checklist

- [ ] Mahmoud Ragab appears as the logged-in user; no alternate logged-in identity is present.
- [ ] Existing token variables are used.
- [ ] Sidebar remains 248px and topbar remains 66px on supported desktop layouts.
- [ ] Tables preserve numeric alignment, row hover, horizontal overflow and semantic badges.
- [ ] Search/filter changes reset pagination where implemented.
- [ ] Loading skeletons, empty results and disabled pagination are tested.
- [ ] Every AI write action opens `ApprovalModal`.
- [ ] Cancel and Reject never execute the action.
- [ ] Success feedback appears only after Approve.
- [ ] Unknown mobile, error and AI-loading behavior is verified in Figma before implementation.
- [ ] Keyboard focus, dialog focus trapping and `aria-sort` are tested before release.

---

## 7. Developer handoff tables

### Component dimensions

| Component | Verified dimensions |
|---|---|
| Standard button | 36px minimum height; 14px horizontal padding |
| Icon button | 36 × 36px |
| Input / select | 36px height |
| Badge | 22px minimum height |
| Product icon | 30 × 30px; large 40 × 40px |
| Metric icon | 32 × 32px |
| Standard icon | 16 × 16px |
| Medium icon | 20 × 20px |
| Assistant avatar | 34 × 34px; small 27 × 27px |
| Sidebar avatar | 32 × 32px |
| Assistant panel | 420px wide |
| Approval modal | 570px wide |

### Interaction and UI states

| Pattern | Trigger | Result |
|---|---|---|
| Sort | Select sortable header | Toggle direction or activate new key; reset page 1 |
| Filter | Enter search or select filter | Recompute visible rows; reset page 1 |
| Clear filters | Select clear action | Restore default filter values |
| Paginate | Select page/previous/next | Show five-row page; bounds disabled |
| Open assistant | Select Ask Forge AI | Slide panel in and show scrim |
| Dismiss assistant | Select close or scrim | Slide panel out |
| Review AI action | Select recommendation action | Open `ApprovalModal` |
| Cancel/reject | Select modal action | Close without approving |
| Approve | Select Approve | Invoke approval callback and show success where wired |
| Empty result | Filters return no rows | Show empty state and reset action |
| Initial loading | Page mounts | Show five skeleton rows for 350ms in prototype |

### Verification backlog

| Item | Required action |
|---|---|
| Mobile and tablet layouts | Needs verification in Figma |
| Exact fixed table row heights | Needs verification in Figma |
| Dedicated page error patterns | Needs verification in Figma |
| AI typing/loading/error/retry | Needs verification in Figma |
| Approval pending/error/retry | Needs verification in Figma |
| Chart tooltips and dynamic states | Needs verification in Figma |
| Modal focus trap and Escape behavior | Needs interaction confirmation |