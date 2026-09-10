// @ts-check
const eslint = require('@eslint/js');
const tseslint = require('typescript-eslint');
const angular = require('angular-eslint');

const domainRestriction = {
  group: [
    '@angular/*',
    'rxjs',
    'rxjs/*',
    '@ngrx/*',
    'primeng',
    'primeng/*',
    '@core/*',
    '@data-access/*',
    '@features/*',
    '@shared/*',
    '@layout/*'
  ],
  message: 'domain/ must not depend on Angular, RxJS, NgRx or any other layer.'
};

const featuresRestriction = {
  group: ['@data-access/api/*'],
  message: 'features/ must depend on @data-access contracts, not directly on the api layer.'
};

const FEATURES = ['setup', 'game', 'admin', 'results'];

// One block per feature: ESLint replaces rule options wholesale, so every restriction that must
// apply inside a feature folder has to be declared together in the same block.
// store/ is intentionally exempt from the cross-feature restriction: signalStore instances here
// are root-provided app state (GameStore, CategoryStore), not feature-private implementation
// detail, so other features are allowed to depend on them the same way they depend on core/.
const featureBoundaryConfigs = FEATURES.map((feature) => ({
  files: [`src/app/features/${feature}/**/*.ts`],
  ignores: [`src/app/features/${feature}/**/*.routes.ts`],
  rules: {
    'no-restricted-imports': [
      'error',
      {
        patterns: [
          featuresRestriction,
          {
            group: FEATURES.filter((other) => other !== feature).flatMap((other) => [
              `@features/${other}/pages/*`,
              `@features/${other}/ui/*`,
              `@features/${other}/*.routes`
            ]),
            message: 'Cross-feature imports are not allowed.'
          }
        ]
      }
    ]
  }
}));

module.exports = tseslint.config(
  {
    files: ['**/*.ts'],
    extends: [
      eslint.configs.recommended,
      ...tseslint.configs.recommended,
      ...angular.configs.tsRecommended
    ],
    processor: angular.processInlineTemplates,
    rules: {
      '@angular-eslint/component-selector': ['error', { type: 'element', prefix: 'app', style: 'kebab-case' }],
      '@angular-eslint/directive-selector': ['error', { type: 'attribute', prefix: 'app', style: 'camelCase' }],
      '@angular-eslint/prefer-standalone': 'error',
      '@angular-eslint/prefer-on-push-component-change-detection': 'error',
      '@typescript-eslint/no-explicit-any': 'error',
      '@typescript-eslint/explicit-function-return-type': 'warn',
      '@typescript-eslint/no-unused-vars': 'error'
    }
  },
  {
    files: ['src/app/domain/**/*.ts'],
    rules: {
      'no-restricted-imports': ['error', { patterns: [domainRestriction] }]
    }
  },
  {
    files: ['src/app/features/**/*.ts'],
    ignores: ['src/app/features/**/*.routes.ts'],
    rules: {
      'no-restricted-imports': ['error', { patterns: [featuresRestriction] }]
    }
  },
  ...featureBoundaryConfigs,
  {
    files: ['**/*.html'],
    extends: [...angular.configs.templateRecommended],
    rules: {
      '@angular-eslint/template/prefer-control-flow': 'error'
    }
  }
);
