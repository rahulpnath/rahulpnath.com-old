---
layout: post
title: "Subresource Integrity (SRI)"
comments: true
categories:
- Security
tags: 
date: 2018-08-28
completedDate: 2018-08-28 05:05:21 +1000
keywords: 
description: Enable browsers to verify files they fetch (for example, from a CDN) are delivered without manipulation.
primaryImage: sri.png
---

_This article is part of a series of articles - [Ok I have got HTTPS! What Next?](/blog/ok-i-have-got-https-what-next/). In this post, we explore how to use Subresource Integrity and the issues it solves._

[Subresource Integrity (SRI)](https://developer.mozilla.org/en-US/docs/Web/Security/Subresource_Integrity) is a security feature that enables browsers to verify that files they fetch (for example, from a CDN) are delivered without unexpected manipulation. It works by allowing you to provide a cryptographic hash that a fetched file must match.

<img src="{{site.images_root}}/sri.png" class="center" alt="Subresource integrity" />

Using the integrity attribute on [script](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/script) and [link](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/link) element enables browsers to verify externally linked files before loading them. The integrity attribute takes a base64-encoded hash prefixed the corresponding hash algorithm prefix(at present sha256,sha3384, sha512), as shown in the example below.

```javascript Integrity attribute as part of the script tag
<script
  src="https://cdnjs.cloudflare.com/ajax/libs/redux/4.0.0/redux.js"
  integrity="sha256-KLkq+W1kKUA6iR5s5Xa/tdzU0yAmXNu7qIGKR/PBoUE="
  crossorigin="anonymous" />
```
### Generating SRI Hash

To generate the SRI hash for files that are accessible over a URL, you can use [srihash.org](https://www.srihash.org/) or [srigenerator](https://allyoucan.cloud/tools/srigenerator/) depending on what hash algorithm version you want. If you're going to generate it on your local files, you can use OpenSSL command-line tool (which should be part of your git bash shell if you are looking around for it, like I did)

``` bash
openssl dgst -sha256 -binary FILENAME.js | openssl base64 -A
```

### Third-Party Libraries

For third-party libraries (js and CSS) referred via CDN, you can grab the script/link element along with the integrity attribute from the CDN sites. Here is an example below from [cdnjs](https://cdnjs.com/).

<img src="{{site.images_root}}/sri_redux.png" alt="Generate script tag along with SRI Hash" class="center" />

When referring third party libraries via CDN its [good to fall back to a local copy](https://www.hanselman.com/blog/CDNsFailButYourScriptsDontHaveToFallbackFromCDNToLocalJQuery.aspx). In cases where the CDN is unreachable or the integrity check fails it can fall back to a local copy. I chose to include the integrity attribute on the fallback copy as well.

``` javascript
<script>
    window.jQuery || 
    document.write('<script src="{{ root_url }}/javascripts/libs/jquery/jquery-2.0.3.min.js" crossorigin="anonymous" integrity="sha256-ruuHogwePywKZ7bI1vHGGs7ScbBLhkNUcSSeRjhSUko=">\x3C/script>')
</script>
```

### Application Specific Files

For application specific javascript files, you need to generate the hash everytime you modify it. You could look at integrating this with your build pipeline to make it seamless. You can use the OpenSSL command line tool as shown above to generate the hash during your application build process.

### Inline JavaScript

_The [integrity](https://html.spec.whatwg.org/multipage/scripting.html#attr-script-integrity) attribute must not be specified when embedding a module script or when the src attribute is not specified._ This means that SRI cannot be used for inline javascript. Even though inline javascript should be avoided, there still are scenarios where you might use that or have dynamically generated javascript. In these cases, we can use [nonce](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/script-src) attribute on script tag and whitelist that nonce in the [CSP Headers](https://rahulpnath.com/blog/http-content-security-policy-csp/).

> **nonce-<base64-value\>**   
> *A whitelist for specific inline scripts using a cryptographic nonce (number used once). The server must generate a unique nonce value each time it transmits a policy. It is critical to provide an unguessable nonce, as bypassing a resource’s policy is otherwise trivial. See unsafe inline script for example. Specifying nonce makes a modern browser ignore 'unsafe-inline' which could still be set for older browsers without nonce support.*

For the jquery fallback above we need a nonce attribute since this is loaded inline.

``` js Nonce attribute
<script nonce="anF1ZXJ5ZmFsbGJhY2s=">
    window.jQuery || 
    document.write('<script src="{{ root_url }}/javascripts/libs/jquery/jquery-2.0.3.min.js" crossorigin="anonymous" integrity="sha256-ruuHogwePywKZ7bI1vHGGs7ScbBLhkNUcSSeRjhSUko=">\x3C/script>')
</script>
```
We can then specify this nonce on the CSP headers for the script-src. The nonce value can be anything that is base64 encoded.

``` xml Web.config CSP header
<add 
  name="Content-Security-Policy" 
  value="default-src 'self';script-src c.disquscdn.com 'self' 'nonce-anF1ZXJ5ZmFsbGJhY2s=' 'nonce-ZGlzcXVzc2NyaXB0'; />
```

Using nonce allows us to get away with having an inline script. However, this should be avoided if possible. As you may have noticed, by having a nonce on the attribute does not validate the script contents of the associated tag. It executes anything that is within that tag. So if you have dynamic content within the script block, this can be used to your disadvantage by attackers. So use it only if it's absolutely necessary. However, having the nonce attribute for those cases is better so that you can limit inline javascript to those specific script tags.

### Browser Support

[Check if](https://caniuse.com/#search=sri) your browser supports Subresource Integrity. Compared to a while back most of the browsers now support SRI.

<a href="https://caniuse.com/#search=sri">
  <img src="{{site.images_root}}/sri_browser.png" alt="SRI Browser Support" class="center" />
</a>

Using SRI, we can make sure that the dependencies that we have are loaded are as expected and not modified in flight or at source by a malicious attacker. There is always a risk that you need to be willing to take when including external dependencies as they could be already having a threat embedded at the time of hash generation. For popular libraries, this is less likely. For those unpopular ones, it's always a good idea to take a quick look at the code to ensure it's not malicious. Using some tools to assist you with this is also a good idea, which we will look into in a separate article.